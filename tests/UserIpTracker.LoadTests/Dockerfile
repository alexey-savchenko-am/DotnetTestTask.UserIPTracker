FROM python:3.11-slim

WORKDIR /app

COPY ./tests/UserIpTracker.LoadTests/load_test.py .

RUN pip install --no-cache-dir aiohttp

CMD ["python", "load_test.py"]
