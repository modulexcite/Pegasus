﻿// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Tests.Performance
{
    using System.IO;
    using Pegasus.Parser;

    public class PegParserTests : PerformanceTestBase
    {
        private readonly string emptyGrammar;
        private readonly string pegGrammar;

        public PegParserTests()
        {
            this.emptyGrammar = "a = ((())) ((() (() (((()) (() () ()))) ()) () (((((() (( ()) () (( ())))) (() (()))) () () (())) () (() (() () (())) ()) () () (((( ()) ())) ((()) (()) ) () () (((()) ) ()) ) ((() (() ()) ((()) ())))) ) ()) (((() ((() ((()) ()) ())) (() (() (()) () ((())))))) (((() ()))) (() ((() (()) () (()) (() ()))) ()) ()) () ()) ()";
            this.pegGrammar = File.ReadAllText("PegParser.peg");
        }

        [Evaluate]
        public void EmptyGrammar()
        {
            new PegParser().Parse(this.emptyGrammar);
        }

        [Evaluate]
        public void PegGrammar()
        {
            new PegParser().Parse(this.pegGrammar);
        }
    }
}
