using System;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using TarkovPriceViewer.Configuration;

namespace TarkovPriceViewer.Services
{
    public interface IOcrService
    {
        void EnsureInitialized(string language);
        string RecognizeText(Mat textMat, char[] currencySplitChars);
    }

    public class OcrService : IOcrService
    {
        private readonly object _lock = new object();
        private RecognizationModel _languageModel;
        private PaddleOcrRecognizer _ocrRecognizer;

        public void EnsureInitialized(string language)
        {
            if (_ocrRecognizer != null)
            {
                return;
            }

            EnsureModel(language);

            if (_languageModel == null)
            {
                return;
            }

            lock (_lock)
            {
                if (_ocrRecognizer != null)
                {
                    return;
                }

                try
                {
                    _ocrRecognizer = new PaddleOcrRecognizer(_languageModel, PaddleDevice.Gpu());
                }
                catch (Exception e)
                {
                    AppLogger.Error("OcrService.EnsureInitialized", "Error creating PaddleOcrRecognizer GPU", e);
                    try
                    {
                        _ocrRecognizer = new PaddleOcrRecognizer(_languageModel, PaddleDevice.Mkldnn());
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Error("OcrService.EnsureInitialized", "Error creating PaddleOcrRecognizer CPU", ex);
                        _ocrRecognizer = null;
                    }
                }
            }
        }

        private void EnsureModel(string language)
        {
            if (_languageModel != null)
            {
                return;
            }

            try
            {
                RecognizationModel model;
                if (language == "ko")
                {
                    model = LocalDictOnlineRecognizationModel.KoreanV4.DownloadAsync().GetAwaiter().GetResult();
                }
                else if (language == "cn")
                {
                    model = LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync().GetAwaiter().GetResult();
                }
                else if (language == "jp")
                {
                    model = LocalDictOnlineRecognizationModel.JapanV4.DownloadAsync().GetAwaiter().GetResult();
                }
                else
                {
                    model = LocalDictOnlineRecognizationModel.EnglishV4.DownloadAsync().GetAwaiter().GetResult();
                }

                lock (_lock)
                {
                    if (_languageModel == null)
                    {
                        AppLogger.Info("OcrService.EnsureModel", "Ensure PaddleOCR language model (use cached files or download if needed).");
                        _languageModel = model;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("OcrService.EnsureModel", "Error downloading Paddle model", ex);
            }
        }

        public string RecognizeText(Mat textMat, char[] currencySplitChars)
        {
            if (textMat == null)
            {
                return string.Empty;
            }

            if (_ocrRecognizer == null)
            {
                return string.Empty;
            }

            string text = string.Empty;
            try
            {
                lock (_lock)
                {
                    var result = _ocrRecognizer.Run(textMat);
                    if (result.Score > 0.5f)
                    {
                        text = result.Text.Replace("\n", " ").Split(currencySplitChars)[0].Trim();
                    }
                    AppLogger.Info("OcrService.RecognizeText", $"Score={result.Score} Text={result.Text}");
                }
            }
            catch (Exception e)
            {
                AppLogger.Error("OcrService.RecognizeText", "Paddle error", e);
            }

            return text;
        }
    }
}
