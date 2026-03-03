"""
Sentiment analysis service using Hugging Face Transformers.
Loads the model once and provides fast predictions.
"""

import logging
from functools import lru_cache

from transformers import pipeline  # type: ignore

logger = logging.getLogger(__name__)

LABEL_MAP = {
    "POSITIVE": "Positive",
    "NEGATIVE": "Negative",
    "NEUTRAL": "Neutral",
}


@lru_cache(maxsize=1)
def get_sentiment_pipeline():
    """
    Loads the sentiment pipeline once (singleton via lru_cache).
    Uses distilbert-base-uncased-finetuned-sst-2-english — fast and reliable.
    """
    logger.info("Loading sentiment analysis model...")
    pipe = pipeline(
        "sentiment-analysis",
        model="distilbert-base-uncased-finetuned-sst-2-english",
    )
    logger.info("Model loaded successfully!")
    return pipe


def analyze_sentiment(text: str) -> dict:
    """
    Analyzes the sentiment of the provided text.

    Returns:
        Dict with 'label' (Positive/Negative/Neutral) and 'score' (0.0-1.0).
    """
    try:
        pipe = get_sentiment_pipeline()
        result = pipe(text[:512])[0]
    except Exception as e:
        logger.error("Pipeline error: %s", e, exc_info=True)
        raise

    raw_label = result["label"]   # "POSITIVE" or "NEGATIVE"
    score = round(result["score"], 4)

    label = LABEL_MAP.get(raw_label, "Neutral")

    # Treat low-confidence predictions as Neutral
    if score < 0.65:
        label = "Neutral"

    logger.info("Analysis done: label=%s, score=%.4f", label, score)
    return {"label": label, "score": score}
