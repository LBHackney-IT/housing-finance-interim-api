using System.Collections.Generic;
using System.Net;
using Google;
using Google.Apis.Requests;

namespace HousingFinanceInterimApi.Tests.V1.TestHelpers
{
    public static class ErrorGen
    {
        public static GoogleApiException FileNotFoundException(string fileId = "")
            => GoogleApiException("drive", 404, $"File not found: {fileId}.", "notFound");

        public static GoogleApiException StorageQuotaExceeded()
            => GoogleApiException("drive", 403, "The user's Drive storage quota has been exceeded.", "storageQuotaExceeded");

        private static string ExceptionErrorMessage(string serviceName, int httpStatusCode, string errorMessage)
            => $"The service {serviceName} has thrown an exception. HttpStatusCode is {(HttpStatusCode) httpStatusCode}. {errorMessage}";

        private static GoogleApiException GoogleApiException(string serviceName, int httpStatusCode, string errorMessage, string failureReason)
        {
            var exceptionMessage = ExceptionErrorMessage(serviceName, httpStatusCode, errorMessage);
            var googleApiException = new GoogleApiException(serviceName, exceptionMessage);

            var singleError = SingleError(errorMessage, failureReason);
            var requestError = RequestError(httpStatusCode, errorMessage, singleError);

            googleApiException.Error = requestError;
            googleApiException.HttpStatusCode = HttpStatusCode.NotFound;

            return googleApiException;
        }

        private static SingleError SingleError(string errorMessage, string failureReason)
        {
            bool isFileNotFound = errorMessage.Contains("File not found");

            return new SingleError
            {
                Domain = "global",
                Reason = failureReason,
                Message = errorMessage,
                LocationType = isFileNotFound ? "parameter" : "",
                Location = isFileNotFound ? "fileId" : ""
            };
        }

        private static RequestError RequestError(int httpStatusCode, string errorMessage, SingleError singleError)
        {
            return new RequestError()
            {
                // So far none of the encountered cases contained more than 1 error.
                Errors = new List<SingleError> { singleError },
                Code = httpStatusCode,
                Message = errorMessage,
            };
        }
    }
}
