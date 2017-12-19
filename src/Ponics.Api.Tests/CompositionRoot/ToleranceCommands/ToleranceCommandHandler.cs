﻿using System;
using Ponics.Analysis.Levels;
using Ponics.Commands;

namespace Ponics.Api.Tests.CompositionRoot.ToleranceCommands
{
    public class ToleranceCommandHandler<TTolerance> : ICommandHandler<ToleranceCommand<TTolerance>>
        where TTolerance : Tolerance
    {
        public void Handle(ToleranceCommand<TTolerance> command)
        {
            throw new NotImplementedException();
        }
    }
}
