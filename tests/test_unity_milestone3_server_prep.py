from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
GAMESTATE = ROOT / "Assets" / "Game" / "Scripts" / "GameState"
NETWORKING = ROOT / "Assets" / "Game" / "Scripts" / "Networking"
UNITY_DOC = ROOT / "docs" / "vitrial-unity.md"
README = ROOT / "README.md"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def assert_contains(path: Path, terms: list[str]) -> None:
    content = read(path)
    for term in terms:
        assert term in content, f"{path.name} should contain {term}"


def test_game_mode_externalizes_runtime_connection_config():
    assert_contains(
        GAMESTATE / "GameModeDefinition.cs",
        [
            "AuthorityMode authorityMode = AuthorityMode.LocalPrototype",
            "serverHost = \"127.0.0.1\"",
            "serverPort = 7777",
            "secureTransport",
            "autoConnectOnClientLaunch",
            "dedicatedServerBuildTarget = \"LinuxServer\"",
            "ClientConnectionConfig ConnectionConfig",
            "UsesServerAuthority",
        ],
    )


def test_match_bootstrap_exposes_future_client_connection_surface():
    assert_contains(
        GAMESTATE / "MatchBootstrap.cs",
        [
            "ClientConnectionConfig.Localhost",
            "IClientCommandAuthorizer commandAuthorizer",
            "LocalPrototypeAuthorizer",
            "CurrentAuthorityMode",
            "ShouldAttemptClientConnection",
            "gameMode.UsesServerAuthority",
            "gameMode.AutoConnectOnClientLaunch",
            "connectionConfig = gameMode.ConnectionConfig",
        ],
    )


def test_networking_folder_defines_transport_agnostic_authority_seams():
    assert_contains(
        NETWORKING / "ClientConnectionConfig.cs",
        [
            "readonly struct ClientConnectionConfig",
            "Host",
            "Port",
            "SecureTransport",
            "PlayerAlias",
            "Localhost",
        ],
    )
    assert_contains(
        NETWORKING / "IClientCommandAuthorizer.cs",
        [
            "interface IClientCommandAuthorizer",
            "ValidateClientCommand",
            "AuthorityDecision",
        ],
    )
    assert_contains(
        NETWORKING / "LocalPrototypeAuthorizer.cs",
        [
            "sealed class LocalPrototypeAuthorizer",
            "IClientCommandAuthorizer",
            "AuthorityDecision.Accept()",
            "AuthorityDecision.Reject",
        ],
    )


def test_docs_describe_dedicated_server_path_without_overbuilding_networking():
    assert_contains(
        NETWORKING / "README.md",
        [
            "Milestone 3 keeps networking as a seam",
            "Current contracts",
            "Future dedicated-server extension points",
            "Replace or wrap `LocalPrototypeAuthorizer`",
            "MatchBootstrap.ConnectionConfig",
            "LinuxServer",
            "Intentionally deferred",
            "Transport/package selection",
        ],
    )
    assert_contains(
        UNITY_DOC,
        [
            "Milestone 3 client/server seam prep",
            "Initial client connection flow design",
            "AuthorityMode.LocalPrototype",
            "ServerAuthoritative",
            "AutoConnectOnClientLaunch",
            "IClientCommandAuthorizer",
            "Intentionally deferred",
        ],
    )
    assert_contains(
        README,
        [
            "Runtime server prep is intentionally lightweight",
            "GameModeDefinition",
            "MatchBootstrap",
            "transport-agnostic authority seams",
            "does not attempt a network connection",
        ],
    )
