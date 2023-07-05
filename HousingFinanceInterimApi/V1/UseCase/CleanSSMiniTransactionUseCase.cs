using System;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;

namespace HousingFinanceInterimApi.V1.UseCase;

public class CleanSSMiniTransactionUseCase: ICleanSSMiniTransactionUseCase
{
    private readonly IBatchLogGateway _batchLogGateway;
    private readonly IBatchLogErrorGateway _batchLogErrorGateway;
    private readonly ITransactionGateway _transactionGateway;

    public CleanSSMiniTransactionUseCase(
        IBatchLogGateway batchLogGateway,
        IBatchLogErrorGateway batchLogErrorGateway
    )
    {
        _batchLogGateway = batchLogGateway;
        _batchLogErrorGateway = batchLogErrorGateway;
    }

    private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

    private const string _label = "CleanSSMiniTransaction";

    public async Task<StepResponse> ExecuteAsync()
    {
        LoggingHandler.LogInfo("Cleaning SSMiniTransactions Table");
        var batch = await _batchLogGateway.CreateAsync(_label).ConfigureAwait(false);
        try
        {
            await _transactionGateway.CleanSSMiniTransactions().ConfigureAwait(false);
            LoggingHandler.LogInfo("SSMiniTransactions Table Cleaned");
            return new StepResponse()
            {
                Continue = true, NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }
        catch (Exception exc)
        {
            var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

            await _batchLogErrorGateway
                .CreateAsync(batch.Id, "ERROR", $"Application error. Not possible to clean SS Mini Transactions")
                .ConfigureAwait(false);

            LoggingHandler.LogError($"{namespaceLabel} Application error");
            LoggingHandler.LogError(exc.ToString());

            throw;
        }
    }
}
