"""
Teste pentru serviciul de analiză a sentimentului.
"""

from unittest.mock import MagicMock, patch

import pytest

from app.services.sentiment_analyzer import analyze_sentiment, LABEL_MAP


class TestAnalyzeSentiment:
    """Teste pentru funcția analyze_sentiment."""

    @patch("app.services.sentiment_analyzer.get_sentiment_pipeline")
    def test_positive_sentiment(self, mock_pipeline):
        """Testează că un text pozitiv returnează label Positive."""
        mock_pipe = MagicMock()
        mock_pipe.return_value = [{"label": "POSITIVE", "score": 0.95}]
        mock_pipeline.return_value = mock_pipe

        result = analyze_sentiment("Mâncarea a fost excelentă!")

        assert result["label"] == "Positive"
        assert result["score"] == 0.95

    @patch("app.services.sentiment_analyzer.get_sentiment_pipeline")
    def test_negative_sentiment(self, mock_pipeline):
        """Testează că un text negativ returnează label Negative."""
        mock_pipe = MagicMock()
        mock_pipe.return_value = [{"label": "NEGATIVE", "score": 0.92}]
        mock_pipeline.return_value = mock_pipe

        result = analyze_sentiment("Serviciul a fost oribil.")

        assert result["label"] == "Negative"
        assert result["score"] == 0.92

    @patch("app.services.sentiment_analyzer.get_sentiment_pipeline")
    def test_low_score_returns_neutral(self, mock_pipeline):
        """Testează că un scor sub 0.6 returnează Neutral."""
        mock_pipe = MagicMock()
        mock_pipe.return_value = [{"label": "POSITIVE", "score": 0.55}]
        mock_pipeline.return_value = mock_pipe

        result = analyze_sentiment("E ok.")

        assert result["label"] == "Neutral"
        assert result["score"] == 0.55

    def test_label_map_has_expected_values(self):
        """Verifică că maparea etichetelor conține valorile așteptate."""
        assert "POSITIVE" in LABEL_MAP
        assert "NEGATIVE" in LABEL_MAP
        assert LABEL_MAP["POSITIVE"] == "Positive"
        assert LABEL_MAP["NEGATIVE"] == "Negative"
