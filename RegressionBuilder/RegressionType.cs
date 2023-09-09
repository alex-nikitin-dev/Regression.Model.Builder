using System.ComponentModel.DataAnnotations;

namespace RegressionBuilder
{
    public enum RegressionType
    {
        [Display(Name="Regression has not been calculated")]
        None,
        [Display(Name="Linear Regression")]
        Linear,
        [Display(Name="Non-Linear Regression")]
        NonLinear
    }
}
