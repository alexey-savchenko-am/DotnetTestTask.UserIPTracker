FROM python:3.11-slim

WORKDIR /app

COPY ./load-tests/load_test.py .

RUN pip install --no-cache-dir aiohttp

CMD ["python", "load_test.py"]
