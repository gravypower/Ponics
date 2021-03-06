﻿using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ponics.Data.Decorators;
using Ponics.Data.Seed;
using Ponics.HardCodedData.Organisms;
using Ponics.Kernel.Commands;
using Ponics.Kernel.Queries;
using Ponics.Organisms;
using Ponics.Organisms.Commands;
using Ponics.Organisms.Queries;

namespace Ponics.Data.Tests.Decorator
{
    [TestFixture]
    public class SeedOrganismsDecoratorTests
    {
        public SeedOrganismsDecorator Sut;
        private IDataCommandHandler<AddOrganism> _dataCommandHandler;
        private IDataQueryHandler<GetOrganisms, List<Organism>> _dataQueryHandler;
        private SeedData<Organism> _organisms;

        [SetUp]
        public void SetUp()
        {
            _dataQueryHandler = Substitute.For<IDataQueryHandler<GetOrganisms, List<Organism>>>();
            _dataCommandHandler = Substitute.For<IDataCommandHandler<AddOrganism>>();
            _organisms = Substitute.For<SeedData<Organism>>();
            Sut = new SeedOrganismsDecorator(_dataQueryHandler, _dataCommandHandler, _organisms);
        }

        [Test]
        public void GivenOrganisms_ThenPassesThroughResults()
        {
            //Assign
            var query = new GetOrganisms();

            _dataQueryHandler.Handle(query).Returns(
                new List<Organism>
                {
                    new SilverPerch()
                });

            //Act
            var result = Sut.Handle(query);

            //Assert
            _dataQueryHandler.Received().Handle(query);
            result.Should().NotBeEmpty();
        }

        [Test]
        public void GivenNoOrganisms_ThenDataShouldBeSeeded()
        {
            //Assign
            var query = new GetOrganisms();
            _dataQueryHandler.Handle(query).Returns(new List<Organism>());

            var organism = new SilverPerch();
            _organisms.GetSeedData().Returns(new[] {organism});

            //Act
            var result = Sut.Handle(query);

            //Assert
            _dataQueryHandler.Received(2).Handle(query);
            _organisms.Received().GetSeedData();
            _dataCommandHandler.Received().Handle(Arg.Is<AddOrganism>( c => c.Organism == organism));
        }
    }
}
