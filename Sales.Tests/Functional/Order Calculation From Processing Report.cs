using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales.DataAccess;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Functional
{
    [TestFixture()]
    public class Order_Calculation_From_Processing_Report
    {
        [Test()]
        [Description("Manifest with static fixed cost supplied should base order on that")]
        public void ManifestWithStaticCosts()
        {
            var mockProduct = new Mock<Product>();
            mockProduct.Setup(m => m.Key).Returns(DataServiceOperation.PHONE.ToString);
            mockProduct.CallBase = true;
            var product = mockProduct.Object;

            var cost = Cost.DefaultCost(product);
            var rateCard = new RateCard(new[] {cost}, PricingModel.Match);

            var mockCostService = new Mock<ICostService>(MockBehavior.Strict);
            mockCostService.Setup(m => m.CreateRateCard(DataServiceOperation.PHONE, It.IsAny<CancellationToken>())).Returns(Task.FromResult(rateCard));

            var costService = mockCostService.Object;

            var mockClient = new Mock<ClientRef>();

            var manifest = new ManifestBuilder();
            var operation = new OperationDefinition();
            operation.Cost = 1.5m;
            operation.Name = DataServiceOperation.PHONE;
            operation.PricingModel = PricingModel.Match;
            manifest.Operations.Add(operation);

            var report = new ProcessingReport();
            report.TotalRecords = 101;
            var result = new OperationReport(DataServiceOperation.PHONE, MatchLevel.T1, 5);
            report.Operations.Add(result);

            var deal = new MutableDeal(mockClient.Object, Guid.NewGuid());
            var order = deal.CreateOrder();

            var calculator = new StandardOrderCalculationService();
            calculator.FillFromRateCard(order, costService, manifest, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.5m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(5));

            // Switch to perrecord
            order.Lines.Clear();
            operation.PricingModel = PricingModel.Record;
            calculator.FillFromRateCard(order, costService, manifest, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.5m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(101));
        }

        [Test()]
        [Description("Manifest with dynamic cost supplied should base order on that")]
        public void ManifestWithDynamicCosts()
        {
            var mockProduct = new Mock<Product>();
            mockProduct.Setup(m => m.Key).Returns(DataServiceOperation.PHONE.ToString);
            mockProduct.CallBase = true;
            var product = mockProduct.Object;

            var cost = Cost.DefaultCost(product);
            cost.PerMatch = 1.5m;
            cost.PerRecord = 1.6m;
            var rateCard = new RateCard(new[] { cost }, PricingModel.Match);

            var mockCostService = new Mock<ICostService>(MockBehavior.Strict);
            mockCostService.Setup(m => m.CreateRateCard(DataServiceOperation.PHONE, It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(rateCard));

            var costService = mockCostService.Object;

            var mockClient = new Mock<ClientRef>();

            var manifest = new ManifestBuilder();
            var operation = new OperationDefinition();
            operation.Name = DataServiceOperation.PHONE;
            manifest.Operations.Add(operation);
            Assert.That(operation.Cost, Is.EqualTo(0)); // Sanity check

            var report = new ProcessingReport();
            report.TotalRecords = 101;
            var result = new OperationReport(DataServiceOperation.PHONE, MatchLevel.T1, 5);
            report.Operations.Add(result);

            var deal = new MutableDeal(mockClient.Object, Guid.NewGuid());
            var order = deal.CreateOrder();

            var calculator = new StandardOrderCalculationService();
            calculator.FillFromRateCard(order, costService, manifest, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.5m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(5));

            // Switch to perrecord
            order.Lines.Clear();
            rateCard = new RateCard(new[] { cost }, PricingModel.Record);
            calculator.FillFromRateCard(order, costService, manifest, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.6m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(101));
        }

        [Test()]
        [Description("Manifest with reports without matches should still create order with items")]
        public void ReportsWithoutMatchesStillGenerateLines()
        {
            var mockProduct = new Mock<Product>();
            mockProduct.Setup(m => m.Key).Returns(DataServiceOperation.PHONE.ToString);
            mockProduct.CallBase = true;
            var product = mockProduct.Object;

            var cost = Cost.DefaultCost(product);
            cost.PerMatch = 1.5m;
            var rateCard = new RateCard(new[] { cost }, PricingModel.Match);

            var mockCostService = new Mock<ICostService>(MockBehavior.Strict);
            mockCostService.Setup(m => m.CreateRateCard(DataServiceOperation.PHONE, It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(rateCard));

            var costService = mockCostService.Object;

            var mockClient = new Mock<ClientRef>();

            var manifest = new ManifestBuilder();
            var operation = new OperationDefinition();
            operation.Name = DataServiceOperation.PHONE;
            manifest.Operations.Add(operation);
            
            var report = new ProcessingReport();
            report.TotalRecords = 101;
            Assert.That(report.Operations, Has.Count.EqualTo(0)); // Sanity check

            var deal = new MutableDeal(mockClient.Object, Guid.NewGuid());
            var order = deal.CreateOrder();

            var calculator = new StandardOrderCalculationService();
            calculator.FillFromRateCard(order, costService, manifest, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.5m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(0));
        }

        [Test()]
        [Description("Regardless of matches, if there's a file minimum on a rate card, the order will have that too")]
        public void RateCardWithFileMinimumsWillGenerateOrderWithMinimum()
        {
            var mockProduct = new Mock<Product>();
            mockProduct.Setup(m => m.Key).Returns(DataServiceOperation.PHONE.ToString);
            mockProduct.CallBase = true;
            var product = mockProduct.Object;

            var cost = Cost.DefaultCost(product);
            cost.PerMatch = 1.5m;
            cost.FileMinimum = 55m;

            var rateCard = new RateCard(new[] { cost }, PricingModel.Match);
            
            var mockCostService = new Mock<ICostService>(MockBehavior.Strict);
            mockCostService.Setup(m => m.CreateRateCard(DataServiceOperation.PHONE, It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(rateCard));

            var costService = mockCostService.Object;

            var mockClient = new Mock<ClientRef>();

            var manifest = new ManifestBuilder();
            var operation = new OperationDefinition();
            operation.Name = DataServiceOperation.PHONE;
            manifest.Operations.Add(operation);

            var report = new ProcessingReport();
            report.TotalRecords = 101;
            Assert.That(report.Operations, Has.Count.EqualTo(0)); // Sanity check

            var deal = new MutableDeal(mockClient.Object, Guid.NewGuid());
            var order = deal.CreateOrder();

            var calculator = new StandardOrderCalculationService();
            calculator.FillFromRateCard(order, costService, manifest, report).GetAwaiter().GetResult();

            Assert.That(order.OrderMinimum, Is.EqualTo(55));
        }

        [Test()]
        [Description("Just processing report supplied should base order on that")]
        public void ProcessingReportOnly()
        {
            var mockProduct = new Mock<Product>();
            mockProduct.Setup(m => m.Key).Returns(DataServiceOperation.PHONE.ToString);
            mockProduct.CallBase = true;
            var product = mockProduct.Object;

            var cost = Cost.DefaultCost(product);
            cost.PerMatch = 1.5m;
            var rateCard = new RateCard(new[] { cost }, PricingModel.Match);

            var mockCostService = new Mock<ICostService>(MockBehavior.Strict);
            mockCostService.Setup(m => m.CreateRateCard(DataServiceOperation.PHONE, It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(rateCard));

            var costService = mockCostService.Object;

            var mockClient = new Mock<ClientRef>();

            var report = new ProcessingReport();
            report.TotalRecords = 101;
            var result = new OperationReport(DataServiceOperation.PHONE, MatchLevel.T1, 5);
            report.Operations.Add(result);

            var deal = new MutableDeal(mockClient.Object, Guid.NewGuid());
            var order = deal.CreateOrder();

            var calculator = new StandardOrderCalculationService();
            calculator.FillFromRateCard(order, costService, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.5m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(5));

            // Switch to perrecord
            order.Lines.Clear();
            rateCard = new RateCard(new[] { cost }, PricingModel.Record);
            cost.PerRecord = 1.5m;

            calculator.FillFromRateCard(order, costService, report).GetAwaiter().GetResult();

            Assert.That(order.Lines, Is.Not.Empty);
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Price, Is.EqualTo(1.5m));
            Assert.That(order.Lines.First().Product, Is.SameAs(product));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(101));
        }
    }
}
