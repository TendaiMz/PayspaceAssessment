using FakeItEasy;
using MediatR;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaxCalculator.Domain.Commands;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Domain.Repository;

namespace TaxCalculator
{
    [TestFixture]
    public class CalculateIncomeTaxCommandHandlerTest
    {
        private  CalculateIncomeTaxCommand command;
        private IRepository<TaxCalculationType> taxCalculationTypeRepo;
        private IRepository<ProgessiveIncomeTaxRate> progressiveTaxRepo;
        private IRepository<IncomeTax> incomeTaxRepo;
        private CancellationToken cancellationToken;

        public CalculateIncomeTaxCommandHandlerTest()
        {
            command = new CalculateIncomeTaxCommand { AnnualIncome = 20000, PostCode = "7441" };
            cancellationToken = new CancellationToken();
            taxCalculationTypeRepo = A.Fake<IRepository<TaxCalculationType>>();
            progressiveTaxRepo = A.Fake<IRepository<ProgessiveIncomeTaxRate>>();
            incomeTaxRepo = A.Fake<IRepository<IncomeTax>>();

        }


        [Test]
        public void ShouldThrowExceptionIfTaxCalculationTypeRepositoryIsNull()
        {
            //Arrange          
            IRequestHandler<CalculateIncomeTaxCommand, IncomeTax> calculateIncomeTaxCommandHandler;
            taxCalculationTypeRepo = null;

            //Act
            void action() => calculateIncomeTaxCommandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);


            //Assert

            Assert.Throws<ArgumentNullException>(action);

        }

        [Test]
        public void ShouldThrowExceptionIfProgessiveIncomeTaxRateRepositoryIsNull()
        {
            //Arrange          
            IRequestHandler<CalculateIncomeTaxCommand, IncomeTax> calculateIncomeTaxCommandHandler;
            progressiveTaxRepo = null;

            //Act
            void action() => calculateIncomeTaxCommandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);


            //Assert

            Assert.Throws<ArgumentNullException>(action);

        }
        [Test]
        public void ShouldThrowExceptionIfIncomeTaxRepositoryIsNull()
        {
            //Arrange          
            IRequestHandler<CalculateIncomeTaxCommand, IncomeTax> calculateIncomeTaxCommandHandler;
            incomeTaxRepo = null;

            //Act
            void action() => calculateIncomeTaxCommandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);


            //Assert

            Assert.Throws<ArgumentNullException>(action);

        }

        [TestCase(0, 0)]
        [TestCase(5000, 500)]
        [TestCase(15000,2250)]       
        [TestCase(50000, 12500)]
        [TestCase(100000, 28000)]
        [TestCase(200000, 66000)]
        [TestCase(500000, 165000)]
        [Test]
        public async Task ShouldReturnTheCorrectProgressiveIncomeTax(decimal income,decimal tax)
        {
            //Arrange   
            command = new CalculateIncomeTaxCommand { AnnualIncome = income, PostCode = "7441" };

            IncomeTax incomeTax = new IncomeTax { IncomeAmount = income, PostCode = "7441", Tax = tax };


            TaxCalculationType taxCalculationType = new TaxCalculationType { ID = 1, Name = "Progressive", PostCode = "7441" };

            var incomeTaxTypeCollection = new List<TaxCalculationType> { taxCalculationType };
            var incomeTaxCollection = new List<IncomeTax> { incomeTax };

            A.CallTo(() => taxCalculationTypeRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxTypeCollection.AsEnumerable());
            });


            A.CallTo(() => incomeTaxRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxCollection.AsEnumerable());
            });


            var commandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);
            //Act
            var result = await commandHandler.Handle(command, cancellationToken);

            //Assert          
            Assert.AreEqual(incomeTax.IncomeAmount, result.IncomeAmount);
            Assert.AreEqual(incomeTax.PostCode, result.PostCode);
            Assert.AreEqual(incomeTax.Tax, result.Tax);


        }



        [TestCase(0, 0)]
        [TestCase(1000, 17500)]   
        [Test]
     
        public async Task ShouldReturnTheCorrectFlatRateIncomeTax(decimal income, decimal tax)
        {
            //Arrange   
            command = new CalculateIncomeTaxCommand { AnnualIncome = income, PostCode = "7000" };
            IncomeTax incomeTax = new IncomeTax { IncomeAmount = income, PostCode = "7000", Tax = tax };
            TaxCalculationType taxCalculationType = new TaxCalculationType { ID = 1, Name = "Flat rate", PostCode = "7000" };

            var incomeTaxTypeCollection = new List<TaxCalculationType> { taxCalculationType };
            var incomeTaxCollection = new List<IncomeTax> { incomeTax };

            A.CallTo(() => taxCalculationTypeRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxTypeCollection.AsEnumerable());
            });


            A.CallTo(() => incomeTaxRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxCollection.AsEnumerable());
            });


            var commandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);
            //Act
            var result = await commandHandler.Handle(command, cancellationToken);

            //Assert          
            Assert.AreEqual(incomeTax.IncomeAmount, result.IncomeAmount);
            Assert.AreEqual(incomeTax.PostCode, result.PostCode);
            Assert.AreEqual(incomeTax.Tax, result.Tax);

        }


        [TestCase(0, 0)]
        [TestCase(50000, 10000)]        
        [TestCase(250000,12500)]
        [Test]
        public async Task ShouldReturnTheCorrectFlatValueIncomeTax(decimal income, decimal tax)
        {
            //Arrange 
            command = new CalculateIncomeTaxCommand { AnnualIncome = income, PostCode = "A100" };

            IncomeTax incomeTax = new IncomeTax { IncomeAmount = income, PostCode = "A100", Tax = tax };


            TaxCalculationType taxCalculationType = new TaxCalculationType { ID = 1, Name = "Flat Value", PostCode = "A100" };

            var incomeTaxTypeCollection = new List<TaxCalculationType> { taxCalculationType };
            var incomeTaxCollection = new List<IncomeTax> { incomeTax };

            A.CallTo(() => taxCalculationTypeRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxTypeCollection.AsEnumerable());
            });


            A.CallTo(() => incomeTaxRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxCollection.AsEnumerable());
            });


            var commandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);
            //Act
            var result = await commandHandler.Handle(command, cancellationToken);

            //Assert          
            Assert.AreEqual(incomeTax.IncomeAmount, result.IncomeAmount);
            Assert.AreEqual(incomeTax.PostCode, result.PostCode);
            Assert.AreEqual(incomeTax.Tax, result.Tax);
        }


        [Test]
        public async Task ShouldCallSaveIncomeTax()
        {
            //Arrange
            IncomeTax incomeTax = new IncomeTax { IncomeAmount = 20000, PostCode = "7441", Tax = 1000 };


            TaxCalculationType taxCalculationType = new TaxCalculationType { ID = 1, Name = "Name", PostCode = "7441" };

            var incomeTaxTypeCollection = new List<TaxCalculationType> { taxCalculationType };
            var incomeTaxCollection = new List<IncomeTax> { incomeTax };

            A.CallTo(() => taxCalculationTypeRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxTypeCollection.AsEnumerable());
            });


            A.CallTo(() => incomeTaxRepo.GetAllAsync(A<CancellationToken>._)).ReturnsLazily(x =>
            {
                return Task.FromResult(incomeTaxCollection.AsEnumerable());
            });

            var commandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);

            //Act
            var result = await commandHandler.Handle(command, cancellationToken);

            //Assert          
            A.CallTo(() => incomeTaxRepo.SaveAsync(A<IncomeTax>._, A<CancellationToken>._)).MustHaveHappened();

        }



        [Test]
        public void ShouldThrowExceptionIfndHandlerIsCalledWithTaxCalculationTypesOfNull()
        {
            incomeTaxRepo = A.Fake<IRepository<IncomeTax>>();
            //Arrange          

            TaxCalculationType taxCalculationType = new TaxCalculationType { ID = 1, Name = "Name", PostCode = "7441" };

            var incomeTaxTypeCollection = new List<TaxCalculationType> { taxCalculationType };


            A.CallTo(() => taxCalculationTypeRepo.GetAllAsync(A<CancellationToken>._)).ThrowsAsync(x =>
            {
                return new ArgumentNullException();
            });

            var commandHandler = new CalculateIncomeTaxCommandHandler(taxCalculationTypeRepo, progressiveTaxRepo, incomeTaxRepo);

            //Act           
            void action() => commandHandler.Handle(command, cancellationToken).GetAwaiter().GetResult();


            //Assert
            Assert.Throws<ArgumentNullException>(action);

        }

    }
}
