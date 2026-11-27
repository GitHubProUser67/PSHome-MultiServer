"""Exceptions raised by the client library. Tests assert on replies; the library only raises when it cannot get one."""


class ClientError(Exception):
    """Base class of every error raised by pshclient."""


class ReplyTimeoutError(ClientError):
    """The server did not send the awaited message in time. The text names the message."""


class ConnectionClosedError(ClientError):
    """The server closed the connection while the client was waiting for a message. The text names the message."""


class DecodeError(ClientError):
    """A message arrived but its bytes do not have the layout the client expects. The text names the message."""


class LoginFailedError(ClientError):
    """A step of the login path did not succeed (raised by the whole-path helpers); the text names the step."""
