# Writing tests

These tests state what the server must do for a client, one rule per test. Someone who has never seen the protocol should
be able to read a test file top to bottom and know what is being checked and why.

## Layers

| Layer | Where | Contains | Does not contain |
|---|---|---|---|
| Protocol library | `shared/pshclient/` | message encoders/decoders, typed replies, clients with named methods | assertions, title values, printing |
| Title values | `Vita/<Title>/<title>_constants.py` | the values passed into `pshclient`, the title's account rule | test logic |
| Fixtures | `conftest.py` | server (external or started), tickets, logged-in clients, a factory of independent players, a ready game room | assertions |
| Tests | `Vita/<Title>/test_<area>.py` | one rule per test, plain `assert` | sockets, byte offsets, retries |

## Rules

- **Names say what things are.** `lobby`, `reply`, `world_id`, `connection`; no single letters except loop indexes, no
  abbreviations such as `jr`, `sk`, `got`.
- **Replies are dataclasses**, not dicts or tuples: `reply.connection.world_id`, not `jr["conn"]["world"]`.
- **Each encoder/decoder has a docstring with the wire layout** (offset, type, field), in protocol terms.
- **The library raises, the test asserts.** A timeout, a closed connection or a reply that does not decode raises a
  `pshclient` exception with a message that names the message type. Protocol checks that decide pass/fail are `assert`s in
  the test, not in the library.
- **A test name is the rule.** `test_join_reply_points_at_the_created_game`, not `test_j4`. The docstring, if any, says why
  the client needs it.
- **No custom verdicts or summaries.** No PASS/FAIL/INFO counters, no SUMMARY lines, no row labels. pytest reports.
- **Logging, not print.** `log = logging.getLogger(__name__)`; one DEBUG line per message sent and received (direction,
  message name, length). Shown with `pytest --log-cli-level=DEBUG`.
- **Setup lives in fixtures.** A test that needs a logged-in client takes a fixture; it does not log in itself, unless
  login is what it tests.
- **Title values come from `<title>_constants.py`** and are passed into `pshclient` as arguments.
- **Portable.** `pathlib` for paths, no shell, no `/proc`, no signals; bind and connect on `127.0.0.1`; works on Linux and
  Windows.
- **Standard library at run time.** pytest and ruff are the only dependencies (`.[test]`); anything else needs a reason in
  the pull request.
- **A scripted client behaves like the game.** It sends the messages the game sends, in its order, with its field values;
  it never sends a message the game does not (for example SET_RECV_FLAG) to make a test pass. When the game's own
  sequence cannot reach a state, the test checks what the game observes there and says so.
- **Waits are polls with a deadline.** A test waits for a server event with `wait_until(players, condition, deadline,
  what)` (or a named poll in the library); no bare `sleep`, and every wait names what it waits for.
- **Several clients: independent and torn down.** Players come from a factory fixture that closes every socket of every
  player when the test ends, pass or fail; a test never relies on state another test left on the server (unique names).
- **Small functions.** If a function needs a comment to separate its steps, split it.

## Example

```python
def test_join_reply_points_at_the_created_game(host, guest):
    """The client connects to the game server named in the join reply, so it must be the game the host created."""
    world_id = host.create_game(name="join test")

    reply = guest.join_game(world_id)

    assert reply.status == MediusStatus.SUCCESS
    assert reply.connection.world_id & 0xFFFF == world_id & 0xFFFF
```

## Before sending a change

    ruff format .
    ruff check .
    pytest
