"""Synthetic NP (PlayStation Network) tickets: structurally valid, with a made-up signature. For tests only.

A ticket is `header, body field, footer field`. Fields are `u16 datatype, u16 size, data`, big endian; datatypes are
Empty 0, UInt 1, ULong 2, String 4, Time 7, Binary 8, body 0x3000, footer 0x3002, DateOfBirth 0x3011, Age 0x3010.

Version 4.0 (the PS Vita layout): header `u16 0x4000, u32 0, u32 length` where length counts the bytes that follow.
Version 2.1: header `u16 0x2101, u32 0, u16 length`.

Body fields, in order: [4.0 only: u16 0], serial (Binary), issuer (UInt), issued time (Time, ms since 1970), expiry time
(Time), user id (ULong), online id (String, 32 bytes), country (Binary), domain (String), service id (Binary,
"UP9000-<title id>_00"). Then in 4.0: a DateOfBirth field, `u32 status header, u16 age, u16 status`, an Age field.
In 2.1:
`u32 status header, u16 age, u16 status, u32 status duration, u32 date of birth` as raw bytes.
Footer: signature identifier (Binary) and DER signature (Binary; in 4.0 exactly 32 bytes so the footer is 0x30 bytes).
"""

import struct
import time
from typing import Optional

_UINT, _ULONG, _STRING, _TIME, _BINARY, _BODY, _FOOTER = 1, 2, 4, 7, 8, 0x3000, 0x3002
_DATE_OF_BIRTH, _AGE = 0x3011, 0x3010
VERSIONS = ("2.1", "4.0")
HOUR_MS = 3600 * 1000
# The made-up ECDSA (r, s) of every synthetic ticket's signature; see build_ticket.
_FAKE_R = 0x1D0E6B1F4A3C5D7E8F9A0B1C2D3E4F5061728394A5B6C7D
_FAKE_S = 0x0F1E2D3C4B5A69788796A5B4C3D2E1F00112233445566778


def _field(datatype: int, data: bytes) -> bytes:
    return struct.pack(">HH", datatype, len(data)) + data


def _padded(text: str, length: int) -> bytes:
    return text.encode() + b"\0" * (length - len(text))


def _der_integer(value: int) -> bytes:
    data = value.to_bytes((value.bit_length() + 8) // 8, "big")
    return b"\x02" + bytes([len(data)]) + data


def _der_signature(r: int, s: int) -> bytes:
    body = _der_integer(r) + _der_integer(s)
    return b"\x30" + bytes([len(body)]) + body


def _body(online_id: str, title_id: str, issued_ms: int, expires_ms: int, user_id: int, version: str) -> bytes:
    is_v4 = version == "4.0"
    parts = [struct.pack(">H", 0)] if is_v4 else []
    parts += [
        _field(_BINARY, bytes(range(0x10, 0x30))),
        _field(_UINT, struct.pack(">I", 0x100)),
        _field(_TIME, struct.pack(">Q", issued_ms)),
        _field(_TIME, struct.pack(">Q", expires_ms)),
        _field(_ULONG, struct.pack(">Q", user_id)),
        _field(_STRING, _padded(online_id, 0x20)),
        _field(_BINARY, _padded("gb", 4)),
        _field(_STRING, _padded("a1", 4)),
        _field(_BINARY, _padded(f"UP9000-{title_id}_00", 0x18)),
    ]
    if is_v4:
        parts += [
            _field(_DATE_OF_BIRTH, struct.pack(">I", 0x07D00101)),
            struct.pack(">IHH", 0, 20, 0),
            _field(_AGE, struct.pack(">I", 0)),
        ]
    else:
        parts.append(struct.pack(">IHHII", 0, 20, 0, 0, 0x07D00101))
    return b"".join(parts)


def build_ticket(
    online_id: str,
    title_id: str,
    issued_ms: Optional[int] = None,
    expires_ms: Optional[int] = None,
    version: str = "4.0",
    user_id: int = 0x1234567890,
) -> bytes:
    """A ticket for `online_id` on the title with nine-character `title_id`. By default it was issued an hour ago and
    expires in an hour; pass `issued_ms` / `expires_ms` (ms since 1970) for other validity windows."""
    if version not in VERSIONS:
        raise ValueError(f"ticket version {version!r}, expected one of {VERSIONS}")
    now = int(time.time() * 1000)
    issued_ms = now - HOUR_MS if issued_ms is None else issued_ms
    expires_ms = now + HOUR_MS if expires_ms is None else expires_ms
    # A placeholder, not a real signature: a real ticket ends in the issuer's ECDSA signature, a DER-encoded (r, s)
    # pair. _FAKE_R and _FAKE_S are arbitrary patterned numbers of a plausible size, signed with no key, so the footer
    # only has the right shape. It works because Horizon verifies a PSN ticket's signature only when PSN signing keys
    # are configured, and the test server has none; it checks the structure and the issued/expiry window instead.
    # A server that holds the real keys rejects these tickets.
    signature = _der_signature(_FAKE_R, _FAKE_S)
    if version == "4.0":
        # The 4.0 footer holds exactly 32 signature bytes. Any 32 bytes will do since nothing verifies them; this slice
        # is not valid DER.
        signature = signature[8:40]
    footer = _field(_BINARY, b"SCEA") + _field(_BINARY, signature)
    rest = _field(_BODY, _body(online_id, title_id, issued_ms, expires_ms, user_id, version)) + _field(_FOOTER, footer)
    if version == "4.0":
        return struct.pack(">HII", 0x4000, 0, len(rest)) + rest
    return struct.pack(">HIH", 0x2101, 0, len(rest)) + rest
