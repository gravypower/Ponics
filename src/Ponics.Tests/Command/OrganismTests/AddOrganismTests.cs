﻿using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ponics.Kernel.Commands;
using Ponics.Organisms;
using Ponics.Organisms.Commands;

namespace Ponics.Tests.Command.OrganismTests
{
    [TestFixture]
    public class AddOrganismTests
    {
        public AddOrganismCommandHandler Sut;
        private IDataCommandHandler<AddOrganism> _addOrganismDataCommandHandler;

        [SetUp]
        public void SetUp()
        {
            _addOrganismDataCommandHandler = Substitute.For<IDataCommandHandler<AddOrganism>>();
            Sut = new AddOrganismCommandHandler(_addOrganismDataCommandHandler);
        }

        [Test]
        public void OrganismAdded()
        {
            //Assign
            var command = new AddOrganism
            {
                Organism = new Organisms.Organism()
            };

            //Act
            Sut.Handle(command);

            //Assert
            _addOrganismDataCommandHandler.Received().Handle(Arg.Any<AddOrganism>());
        }

        [Test]
        public void OrganismAdded_Id_Generated()
        {
            //Assign
            var command = new AddOrganism
            {
                Organism = new Organisms.Organism()
            };

            //Act
            Sut.Handle(command);

            //Assert
            command.Organism.Id.Should().NotBe(Guid.Empty);
        }

    }
}
