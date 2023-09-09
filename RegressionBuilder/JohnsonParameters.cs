using System;
using System.Collections.Generic;

namespace RegressionBuilder
{
    public class JohnsonParameters
    {
        public Dictionary<JohnsonParameterNames, JohnsonParameter> Parameters { get; }

        private decimal _sampleMin;
        private decimal _sampleMax;

        public decimal Approach { get; set; }

        public bool NeedVerifying
        {
            set
            {
                foreach (var parameter in Parameters)
                {
                    parameter.Value.NeedVerifying = value;
                }
            }
        }
        public JohnsonParameter Phi
        {
            get => Parameters[JohnsonParameterNames.Phi];
            private set => Parameters[JohnsonParameterNames.Phi] = value;
        }

        public JohnsonParameter Lambda
        {
            get => Parameters[JohnsonParameterNames.Lambda];
            private set => Parameters[JohnsonParameterNames.Lambda] = value;
        }

        public JohnsonParameter Gamma
        {
            get => Parameters[JohnsonParameterNames.Gamma];
            private set => Parameters[JohnsonParameterNames.Gamma] = value;
        }

        public JohnsonParameter Eta
        {
            get => Parameters[JohnsonParameterNames.Eta];
            private set => Parameters[JohnsonParameterNames.Eta] = value;
        }
        public void SetValues(JohnsonParameters jParams)
        {
            Phi.Value = jParams.Phi.Value;
            Lambda.Value = jParams.Lambda.Value;
            Gamma.Value = jParams.Gamma.Value;
            Eta.Value = jParams.Eta.Value;
        }

        private JohnsonParameters(decimal sampleMin, decimal sampleMax, decimal approach = 0.001M)
        {
            Parameters = new Dictionary<JohnsonParameterNames, JohnsonParameter>
            {
                {JohnsonParameterNames.Phi, null},
                {JohnsonParameterNames.Lambda, null},
                {JohnsonParameterNames.Gamma, null},
                {JohnsonParameterNames.Eta, null}
            };

            _sampleMin = sampleMin;
            _sampleMax = sampleMax;
            Approach = approach;
        }
        public JohnsonParameters(Sample sample, int steps = 20,bool needVerifying = true,
            decimal approach = 0.001M)
            : this(sample.Min,sample.Max,approach)
        {
            Balance(sample.Min, sample.Max, steps, needVerifying, approach);
        }
        public JohnsonParameters(decimal sampleMin, decimal sampleMax, bool needVerifying = true, int steps = 20,
            decimal approach = 0.001M)
            : this(sampleMin,sampleMax,approach)
        {
            Balance(sampleMin, sampleMax, steps, needVerifying, approach);
        }
        public JohnsonParameters(decimal sampleMin, decimal sampleMax, JohnsonParameter phi, JohnsonParameter lambda, JohnsonParameter gamma,
            JohnsonParameter eta, decimal approach = 0.001M)
            : this(sampleMin, sampleMax, approach)
        {
            Phi = phi;
            Lambda = lambda;
            Gamma = gamma;
            Eta = eta;
            
            VerifyBalance();
        }
        private JohnsonParameters(JohnsonParameters origin)
            : this(origin._sampleMin, origin._sampleMax, origin.Approach)
        {
            foreach (var parameter in Parameters)
            {
                Parameters[parameter.Key] = origin.Parameters[parameter.Key].Clone();
            }
            VerifyBalance();
        }
        public JohnsonParameters(decimal sampleMin, decimal sampleMax, List<JohnsonParameter> jParams, decimal approach = 0.001M)
            : this(sampleMin, sampleMax, approach)
        {
            foreach (var jParam in jParams)
            {
                switch (jParam.Name)
                {
                    case JohnsonParameterNames.Phi:
                        Phi = jParam;
                        break;
                    case JohnsonParameterNames.Gamma:
                        Gamma = jParam;
                        break;
                    case JohnsonParameterNames.Lambda:
                        Lambda = jParam;
                        break;
                    case JohnsonParameterNames.Eta:
                        Eta = jParam;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            VerifyBalance();
        }

        public void Balance(decimal sampleMin, decimal sampleMax, int? steps = null, bool needVerifying = true, decimal approach = 0.001M)
        {
            _sampleMax = sampleMax;
            _sampleMin = sampleMin;
            Approach = approach;

            var min = Approach;//sampleMin / 2M;
            var max = sampleMin;
            var initialVal = min;
            Phi = new(JohnsonParameterNames.Phi, min, max, initialVal, (max - min) / (steps ?? Phi.StepsCount), needVerifying);

            min = sampleMax - Phi.Min;
            max = min * 2M;
            initialVal = min + Approach;
            Lambda = new (JohnsonParameterNames.Lambda, min, max, initialVal, (max - min) / (steps ?? Lambda.StepsCount), needVerifying);

            min = 0;
            max = 2;
            initialVal = min + Approach;
            Gamma = new (JohnsonParameterNames.Gamma, min, max, initialVal, (max - min) / (steps ?? Gamma.StepsCount), needVerifying);

            min = 0;
            max = 2;
            initialVal = min + Approach;
            Eta = new (JohnsonParameterNames.Eta, min, max, initialVal, (max - min) / (steps ?? Eta.StepsCount), needVerifying);

            VerifyBalance();
        }

        private void VerifyBalance()
        { 

#if XMAS

#else
             if (Phi.Max > _sampleMin)
                throw new ArgumentException($@"Phi.Max ({Phi.Max}) > _sampleMin ({_sampleMin})");
            if(Lambda.Min < _sampleMax - Phi.Min)
                throw new ArgumentException($@"Lambda.Min ({Lambda.Min}) < _sampleMax ({_sampleMax}) - Phi.Min ({Phi.Min})");
            if(Eta.Min < 0)
                throw new ArgumentException($@"Eta.Min ({Eta.Min}) < 0");
#endif
           
        }

        public JohnsonParameters Clone()
        {
            return new JohnsonParameters(this);
        }

        public override string ToString()
        {
            return $"Phi = {Phi.Value} {Environment.NewLine}" +
                   $"Lambda = {Lambda.Value} {Environment.NewLine}" +
                   $"Gamma = {Gamma.Value} {Environment.NewLine}" +
                   $"Eta = {Eta.Value} {Environment.NewLine}";
        }

        public List<(string name, string val)> GetListParameters(int precision)
        {
            return new()
            {
                ("Phi",$"{Math.Round(Phi.Value,precision):G29}"),
                ("Lambda",$"{Math.Round(Lambda.Value,precision):G29}"),
                ("Gamma",$"{Math.Round(Gamma.Value,precision):G29}"),
                ("Eta",$"{Math.Round(Eta.Value,precision):G29}")
            };
        }
    }
}
