using System.ComponentModel.DataAnnotations;

namespace RegressionBuilder
{
    public enum SampleNormalizationType
    {
        [Display(Name="Without Normalization")]
        None,
        [Display(Name="Johnson")]
        Johnson,
        [Display(Name="Logarithm 10")]
        Log10,
        [Display(Name="Logarithm E")]
        Log
    }
}
