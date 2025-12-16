from __future__ import annotations

import asyncio
import time
import random
import uuid
from typing import Any

import aiohttp
from aiohttp import ClientSession
from asyncio import Semaphore

TOTAL_REQUESTS = 100
CONCURRENCY = 20
API_URL = "http://useriptracker.api:8080/api/users/connections"

JsonDict = dict[str, Any]

async def worker(session: ClientSession, sem: Semaphore, idx: int) -> None:
    async with sem:
        payload: JsonDict = {
            "userId": str(uuid.uuid4()),
            "ip": f"124.56.{idx % 255}.{random.randint(0, 255)}"
        }
		
        async with session.post(API_URL, json=payload) as response:
            if response.status > 400:
                print(f"ERROR {response.status}")

async def main() -> None:
    semaphore: Semaphore = Semaphore(CONCURRENCY)
	
    async with ClientSession() as session:
        start_time: float = time.perf_counter()

        tasks: list[asyncio.Task[None]] = [
            asyncio.create_task(worker(session, semaphore, idx)) for idx in range(TOTAL_REQUESTS)
        ]

        await asyncio.gather(*tasks)

        elapsed: float = time.perf_counter() - start_time

        print(f"Total requests: {TOTAL_REQUESTS}")
        print(f"Elapsed seconds: {elapsed:.2f}")
        print(f"RPS: {TOTAL_REQUESTS / elapsed:.2f}")


if __name__ == "__main__":
    asyncio.run(main())