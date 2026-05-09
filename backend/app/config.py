from __future__ import annotations

from functools import lru_cache
from os import getenv


class Settings:
    def __init__(self) -> None:
        self.app_name = getenv("APP_NAME", "vion-arena-backend")
        self.app_version = getenv("APP_VERSION", "0.1.0")
        self.allowed_origins = [
            origin.strip()
            for origin in getenv("APP_ALLOWED_ORIGINS", "*").split(",")
            if origin.strip()
        ]


@lru_cache
def get_settings() -> Settings:
    return Settings()

