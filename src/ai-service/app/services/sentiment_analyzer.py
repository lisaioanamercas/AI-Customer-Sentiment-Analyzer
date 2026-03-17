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
    Loads the multilingual sentiment pipeline once.
    Uses cardiffnlp/twitter-xlm-roberta-base-sentiment-multilingual.
    Supports English, Romanian, and many others.
    """
    logger.info("Loading multilingual sentiment analysis model...")
    pipe = pipeline(
        "sentiment-analysis",
        model="cardiffnlp/twitter-xlm-roberta-base-sentiment-multilingual",
    )
    logger.info("Model loaded successfully!")
    return pipe


def analyze_sentiment(text: str) -> dict:
    """
    Analyzes the sentiment of the provided text (EN/RO).
    """
    try:
        pipe = get_sentiment_pipeline()
        result = pipe(text[:512])[0]
    except Exception as e:
        logger.error("Pipeline error: %s", e, exc_info=True)
        raise

    # cardiffnlp model returns labels 'negative', 'neutral', 'positive' (lowercase usually)
    # or sometimes 'LABEL_0', 'LABEL_1', 'LABEL_2'
    raw_label = result["label"].upper() 
    score = round(result["score"], 4)

    # Map labels dynamically
    if raw_label in ["POSITIVE", "LABEL_2"]:
        label = "Positive"
    elif raw_label in ["NEGATIVE", "LABEL_0"]:
        label = "Negative"
    else:
        label = "Neutral"

    # Treat low-confidence predictions as Neutral for better UX
    if score < 0.45: # Multi-lingual models can have lower confidence distribution
        label = "Neutral"

    logger.info("Analysis done: text_preview='%s...', label=%s, score=%.4f", text[:30], label, score)
    return {"label": label, "score": score}
