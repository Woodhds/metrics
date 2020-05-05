using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.ML.Data;
using metrics.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;

namespace metrics.ML.Services
{
    public class VkMessageMLService : BackgroundService
    {
        private readonly IVkMessageService _vkMessageService;

        public VkMessageMLService(IVkMessageService vkMessageService)
        {
            _vkMessageService = vkMessageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var messages =
                    ((IEnumerable<VkMessage>) (await _vkMessageService.GetMessages(0, 900))
                        .Data).Where(e => e.MessageCategoryId.HasValue).Select(e => new VkMessageML
                    {
                        Category = e.MessageCategory,
                        Id = e.Id,
                        Text = e.Text,
                        OwnerId = e.Owner_Id
                    });

                var mlContext = new MLContext();
                var trainingDataView = mlContext.Data.LoadFromEnumerable(messages);
                var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(VkMessageML.Category))
                    .Append(mlContext.Transforms.Text.NormalizeText("NormalizedText", nameof(VkMessageML.Text)))
                    .Append(mlContext.Transforms.Text.FeaturizeText("Features", "NormalizedText"))
                    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                mlContext.MulticlassClassification.CrossValidate(trainingDataView, pipeline);
                var trainedModel = pipeline.Fit(trainingDataView);
                var predEngine = mlContext.Model.CreatePredictionEngine<VkMessageML, VkMessagePredict>(trainedModel);

                var message = new VkMessageML
                {
                    Category = "Посуда", Text = @"&#127942; ДРУЗЬЯ, ВСТРЕЧАЙТЕ ОЧЕРЕДНОЙ РОЗЫГРЫШ ОТ НАШЕГО МАГАЗИНА!

&#128293; 3 подарка - 3 победителя!

&#127942; 1 место - Узбекский Казан на 10 Литров
&#127942; 2-е место Ляган Риштан 37 см
&#127942; 3-е место - сертификат на 500 рублей на покупку любого казана в нашем магазине.

&#128073; Условия участия в конкурсе предельно просты:
(УЧАСТВУЮТ УЧАСТНИКИ СО ВСЕЙ РОССИИ И СНГ)

1. Быть подписчиком нашей группы [club92735106|Узбекский казан&#215;Дары Кавказа&#215;Афганский казан]
2. Сделать репост этой записи себе на стену и не удалять до подведения итогов.
3. Поставить Лайк под этой записью &#127873; Итоги подведем в день великого праздника 9 мая &#128073;В прямом эфире в 20:00Победителей выберем при помощи приложения MegaRandom."
                };
                var prediction = predEngine.Predict(message);
                Console.WriteLine(prediction.Category);
                Console.WriteLine(prediction.Score);
                await Task.Delay(1000 * 60 * 60, stoppingToken);
            }
        }
    }
}