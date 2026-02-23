"""
Teste de integrare pentru endpoint-urile API.
"""

from unittest.mock import patch, MagicMock

import pytest
from httpx import ASGITransport, AsyncClient

from app.main import app


@pytest.mark.asyncio
class TestAnalyzeEndpoint:
    """Teste pentru POST /api/analyze."""

    @patch("app.routers.analyze.analyze_sentiment")
    async def test_analyze_returns_sentiment(self, mock_analyze):
        """Testează că endpoint-ul returnează sentiment valid."""
        mock_analyze.return_value = {"label": "Positive", "score": 0.95}

        transport = ASGITransport(app=app)
        async with AsyncClient(transport=transport, base_url="http://test") as client:
            response = await client.post(
                "/api/analyze",
                json={"text": "Mâncarea a fost excelentă!"},
            )

        assert response.status_code == 200
        data = response.json()
        assert data["label"] == "Positive"
        assert data["score"] == 0.95

    @patch("app.routers.analyze.analyze_sentiment")
    async def test_analyze_empty_text_returns_422(self, mock_analyze):
        """Testează că textul gol returnează 422 Unprocessable Entity."""
        transport = ASGITransport(app=app)
        async with AsyncClient(transport=transport, base_url="http://test") as client:
            response = await client.post(
                "/api/analyze",
                json={"text": ""},
            )

        assert response.status_code == 422


class TestHealthEndpoint:
    """Teste pentru GET /health."""

    @pytest.mark.asyncio
    async def test_health_check(self):
        """Testează că health check returnează status healthy."""
        transport = ASGITransport(app=app)
        async with AsyncClient(transport=transport, base_url="http://test") as client:
            response = await client.get("/health")

        assert response.status_code == 200
        data = response.json()
        assert data["status"] == "healthy"
        assert data["service"] == "ai-sentiment"
