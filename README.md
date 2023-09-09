# Regression.Model.Builder
Software for implementing and building a nonlinear regression model for software size estimation for cross-platform development of mobile applications using Xamarin platform (but not limited to this)

## Software Overview
The purpose of this software is to automate the processes of constructing various regression models, estimating code volume, and performing statistical calculations related to software size estimation for cross-platform mobile application development.

## Detailed Features
- Construct a nonlinear regression model for normalized data using the univariate Johnson transformation and calculate the confidence and prediction intervals.
- Construct a nonlinear regression model for normalized data using the decimal logarithm and calculate the confidence and prediction intervals.
- Construct a linear regression model for raw data without normalization and calculate the confidence and prediction intervals.
- Plot the nonlinear regression model for normalized data using the univariate Johnson transformation, display the confidence and prediction intervals on it, and also display the data points on this graph.
- Plot the nonlinear regression model for normalized data using the decimal logarithm, display the confidence and prediction intervals on it, and also display the data points on this graph.
- Plot the linear regression model for raw data without normalization, display the confidence and prediction intervals on it, and also display the data points on this graph.
- Calculate the characteristics of the initial sample.
- Estimate the code volume (number of code lines) according to the entered data (number of classes).
- Build histograms of the initial sample and a graph with its points.
- Review the points of the initial sample represented by numerical values.
- Review the calculated points of the regression model and prediction and confidence intervals in the form of numerical values.
- Save to a file the calculated data of the regression model (regression points, intervals, regression characteristics).
- Save to a file the calculated normalization data (normalized values and Johnson normalization parameters).
- Load from a file the calculated normalization data (normalized values and Johnson normalization parameters).
- Load from a file the initial data.
