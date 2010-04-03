﻿// <copyright file="NormalGamma.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2009 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.Distributions
{
    using System;
    using System.Collections.Generic;
    using Properties;

    /// <summary>
    /// This structure represents the type over which the <see cref="NormalGamma"/> distribution
    /// is defined.
    /// </summary>
    public struct MeanPrecisionPair
    {
        private double mMean;
        private double mPrecision;

        /// <summary>
        /// Constructs a new mean precision pair.
        /// </summary>
        /// <param name="m">The mean of the pair.</param>
        /// <param name="p">The precision of the pair.</param>
        public MeanPrecisionPair(double m, double p)
        {
            mMean = m;
            mPrecision = p;
        }

        /// <summary>
        /// Gets/sets the mean of the pair.
        /// </summary>
        public double Mean
        {
            get { return mMean; }
            set { mMean = value; }
        }

        /// <summary>
        /// Gets/sets the precision of the pair.
        /// </summary>
        public double Precision
        {
            get { return mPrecision; }
            set { mPrecision = value; }
        }
    }

    /// <summary>
    /// <para>The <see cref="NormalGamma"/> distribution is the conjugate prior distribution for the <see cref="Normal"/>
    /// distribution. It specifies a prior over the mean and precision of the <see cref="Normal"/> distribution.</para>
    /// <para>It is parameterized by four numbers: the mean location, the mean scale, the precision shape and the
    /// precision inverse scale.</para>
    /// <para>The distribution NG(mu, tau | mloc,mscale,psscale,pinvscale) = Normal(mu | mloc, 1/(mscale*tau)) * Gamma(tau | psscale,pinvscale).</para>
    /// <para>The following degenerate cases are special: when the precision is known,
    /// the precision shape will encode the value of the precision while the precision inverse scale is positive
    /// infinity. When the mean is known, the mean location will encode the value of the mean while the scale
    /// will be positive infinity. A completely degenerate NormalGamma distribution with known mean and precision is possible as well.</para>
    /// </summary>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to false, all parameter checks can be turned off.</para></remarks>
    public class NormalGamma
    {
        /// <summary>
        /// The location of the mean.
        /// </summary>
        private double _meanLocation;

        /// <summary>
        /// The scale of the mean.
        /// </summary>
        private double _meanScale;

        /// <summary>
        /// The shape of the precision.
        /// </summary>
        private double _precisionShape;

        /// <summary>
        /// The inverse scale of the precision.
        /// </summary>
        private double _precisionInvScale;

        /// <summary>
        /// The distribution's random number generator.
        /// </summary>
        private Random _random;

        /// <summary>
        /// Constructs a NormalGamma distribution.
        /// </summary>
        /// <param name="meanLocation">The location of the mean.</param>
        /// <param name="meanScale">The scale of the mean.</param>
        /// <param name="precShape">The shape of the precision.</param>
        /// <param name="precInvScale">The inverse scale of the precision.</param>
        public NormalGamma(double meanLocation, double meanScale, double precShape, double precInvScale)
        {
            SetParameters(meanLocation, meanScale, precShape, precInvScale);
            _random = new Random();
        }
        
        /// <summary>
        /// Checks whether the parameters of the distribution are valid. 
        /// </summary>
        /// <param name="meanLocation">The location of the mean.</param>
        /// <param name="meanScale">The scale of the mean.</param>
        /// <param name="precShape">The shape of the precision.</param>
        /// <param name="precInvScale">The inverse scale of the precision.</param>
        /// <returns>True when the parameters are valid, false otherwise.</returns>
        private static bool IsValidParameterSet(double meanLocation, double meanScale, double precShape, double precInvScale)
        {
            if (meanScale <= 0.0 || precShape <= 0.0 || precInvScale <= 0.0 
                || Double.IsNaN(meanLocation) || Double.IsNaN(meanScale) || Double.IsNaN(precShape) 
                || Double.IsNaN(precInvScale))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the parameters of the distribution after checking their validity.
        /// </summary>
        /// <param name="meanLocation">The location of the mean.</param>
        /// <param name="meanScale">The scale of the mean.</param>
        /// <param name="precShape">The shape of the precision.</param>
        /// <param name="precInvScale">The inverse scale of the precision.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the parameters don't pass the <see cref="IsValidParameterSet"/> function.</exception>
        private void SetParameters(double meanLocation, double meanScale, double precShape, double precInvScale)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(meanLocation, meanScale, precShape, precInvScale))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            _meanLocation = meanLocation;
            _meanScale = meanScale;
            _precisionShape = precShape;
            _precisionInvScale = precInvScale;
        }

        /// <summary>
        /// A string representation of the distribution.
        /// </summary>
        public override string ToString()
        {
            return "NormalGamma(Mean Location = " + _meanLocation + ", Mean Scale = " + _meanScale +
                        ", Precision Shape = " + _precisionShape + ", Precision Inverse Scale = " + _precisionInvScale + ")";
        }

        /// <summary>
        /// Gets the location of the mean.
        /// </summary>
        public double MeanLocation
        {
            get { return _meanLocation; }
            set { SetParameters(value, _meanScale, _precisionShape, _precisionInvScale); }
        }

        /// <summary>
        /// Gets the scale of the mean.
        /// </summary>
        public double MeanScale
        {
            get { return _meanScale; }
            set { SetParameters(_meanLocation, value, _precisionShape, _precisionInvScale); }
        }

        /// <summary>
        /// Gets the shape of the precision.
        /// </summary>
        public double PrecisionShape
        {
            get { return _precisionShape; }
        }

        /// <summary>
        /// Gets the inverse scale of the precision.
        /// </summary>
        public double PrecisionInverseScale
        {
            get { return _precisionInvScale; }
        }

        /// <summary>
        /// Returns the marginal distribution for the mean of the <see cref="NormalGamma"/> distribution.
        /// </summary>
        /// <returns></returns>
        public StudentT MeanMarginal()
        {
            return new StudentT(_meanLocation, _meanScale * _precisionShape / _precisionInvScale, 2.0 * _precisionShape);
        }

        /// <summary>
        /// Returns the marginal distribution for the precision of the <see cref="NormalGamma"/> distribution.
        /// </summary>
        /// <returns></returns>
        public Gamma PrecisionMarginal()
        {
            return new Gamma(_precisionShape, _precisionInvScale);
        }

        /*
        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        /// <value>The mean of the distribution.</value>
        public MeanPrecisionPair Mean
        {
            get
            {
                if (Double.IsPositiveInfinity(_precisionInvScale))
                {
                    return new MeanPrecisionPair(_meanLocation, _precisionShape);
                }
                else
                {
                    return new MeanPrecisionPair(_meanLocation, _precisionShape / _precisionInvScale);
                }
            }
        }

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        /// <value>The random number generator used to generate a random sample.</value>
        public System.Random RandomNumberGenerator { get; set; }

        /// <summary>
        /// The mode of the distribution.
        /// </summary>
        /// <value></value>
        public MeanPrecisionPair Mode
        {
            get
            {
                if (Double.IsPositiveInfinity(_precisionInvScale))
                {
                    return new MeanPrecisionPair(_meanLocation, _precisionShape);
                }
                else
                {
                    return new MeanPrecisionPair(_meanLocation, _precisionShape / _precisionInvScale);
                }
            }
        }

        /// <summary>
        /// The median of the distribution.
        /// </summary>
        /// <value></value>
        public MeanPrecisionPair Median
        {
            get
            {
                if (Double.IsPositiveInfinity(_precisionInvScale))
                {
                    return new MeanPrecisionPair(_meanLocation, _precisionShape);
                }
                else
                {
                    return new MeanPrecisionPair(_meanLocation, _precisionShape / _precisionInvScale);
                }
            }
        }

        /// <summary>
        /// Evaluates the probability density function for a NormalGamma distribution.
        /// </summary>
        public double Density(MeanPrecisionPair mp)
        {
            return Density(mp.Mean, mp.Precision);
        }

        /// <summary>
        /// Evaluates the probability density function for a NormalGamma distribution.
        /// </summary>
        public double Density(double mean, double prec)
        {
            if (Double.IsPositiveInfinity(_precisionInvScale) && _meanScale == 0.0)
            {
                throw new NotImplementedException();
            }
            else if (Double.IsPositiveInfinity(_precisionInvScale))
            {
                throw new NotImplementedException();
            }
            else if (_meanScale == 0.0)
            {
                throw new NotImplementedException();
            }
            else
            {
                double e = -0.5 * prec * (mean - _meanLocation) * (mean - _meanLocation) - prec * _precisionInvScale;
                return System.Math.Pow(prec * _precisionInvScale, _precisionShape) * System.Math.Exp(e)
                        / (Math.Constants.Sqrt2Pi * System.Math.Sqrt(prec) * Math.SpecialFunctions.Gamma(_precisionShape));
            }
        }

        /// <summary>
        /// Evaluates the log probability density function for a NormalGamma distribution.
        /// </summary>
        public double DensityLn(MeanPrecisionPair mp)
        {
            return DensityLn(mp.Mean, mp.Precision);
        }

        /// <summary>
        /// Evaluates the log probability density function for a NormalGamma distribution.
        /// </summary>
        public double DensityLn(double mean, double prec)
        {
            if (Double.IsPositiveInfinity(_precisionInvScale) && _meanScale == 0.0)
            {
                throw new NotImplementedException();
            }
            else if (Double.IsPositiveInfinity(_precisionInvScale))
            {
                throw new NotImplementedException();
            }
            else if (_meanScale == 0.0)
            {
                throw new NotImplementedException();
            }
            else
            {
                double e = -0.5 * prec * (mean - _meanLocation) * (mean - _meanLocation) - prec * _precisionInvScale;
                return (_precisionShape - 0.5) * System.Math.Log(prec) + _precisionShape * System.Math.Log(_precisionInvScale) + e
                        - Math.Constants.LogSqrt2Pi - Math.SpecialFunctions.GammaLn(_precisionShape);
            }
        }

        /// <summary>
        /// Samples a NormalGamma distributed random variable.
        /// </summary>
        /// <returns>A random number from this distribution.</returns>
        public MeanPrecisionPair Sample()
        {
            return NormalGamma.Sample(RandomNumberGenerator, _meanLocation, _meanScale, _precisionShape, _precisionInvScale);
        }

        /// <summary>
        /// Samples an array of NormalGamma distributed random variables.
        /// </summary>
        /// <param name="size">The number of variables needed.</param>
        /// <returns>An array of random numbers from this distribution.</returns>
        public MeanPrecisionPair[] Sample(int size)
        {
            return NormalGamma.Sample(RandomNumberGenerator, size, _meanLocation, _meanScale, _precisionShape, _precisionInvScale);
        }

        /// <summary>
        /// Checks the parameters of a NormalGamma distribution.
        /// </summary>
        /// <param name="meanScale">The scale of the mean.</param>
        /// <param name="precShape">The shape of the precision.</param>
        /// <param name="precInvScale">The inverse scale of the precision.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the mean scale is negative.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the inverse precision scale is negative.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the precision shape is negative.</exception>
        private static void CheckParameters(double meanScale, double precShape, double precInvScale)
        {
            if (meanScale < 0.0)
            {
                throw new ArgumentOutOfRangeException("meanScale", Resources.ParameterCannotBeNegative);
            }
            else if (precShape <= 0.0)
            {
                throw new ArgumentOutOfRangeException("precShape", Resources.ParameterCannotBeNegative);
            }
            else if (precInvScale <= 0.0)
            {
                throw new ArgumentOutOfRangeException("precInvScale", Resources.ParameterCannotBeNegative);
            }
        }

        /// <summary>
        /// Samples an array of NormalGamma distributed random variables.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="meanLocation">The location of the mean.</param>
        /// <param name="meanScale">The scale of the mean.</param>
        /// <param name="precShape">The shape of the precision.</param>
        /// <param name="precInvScale">The inverse scale of the precision.</param>
        public static MeanPrecisionPair Sample(System.Random rnd, double meanLocation, double meanScale, double precShape, double precInvScale)
        {
            if (Control.CheckDistributionParameters)
            {
                CheckParameters(meanScale, precShape, precInvScale);
            }

            MeanPrecisionPair mp = new MeanPrecisionPair();

            // Sample the precision.
            if(Double.IsPositiveInfinity(precInvScale))
            {
                mp.Precision = precShape;
            }
            else
            {
                mp.Precision = Gamma.Sample(rnd, precShape, precInvScale);
            }

            // Sample the mean.
            if (meanScale == 0.0)
            {
                mp.Mean = meanLocation;
            }
            else
            {
                mp.Mean = Normal.Sample(rnd, meanLocation, System.Math.Sqrt(meanScale / mp.Precision));
            }

            return mp;
        }

        /// <summary>
        /// Samples an array of NormalGamma distributed random variables.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="n">The number of variables needed.</param>
        /// <param name="meanLocation">The location of the mean.</param>
        /// <param name="meanScale">The scale of the mean.</param>
        /// <param name="precShape">The shape of the precision.</param>
        /// <param name="precInvScale">The inverse scale of the precision.</param>
        public static MeanPrecisionPair[] Sample(System.Random rnd, int n, double meanLocation, double meanScale, double precShape, double precInvScale)
        {
            if (Control.CheckDistributionParameters)
            {
                CheckParameters(meanScale, precShape, precInvScale);
            }

            // First sample all the precisions independently.
            double[] precs = null;
            if (Double.IsPositiveInfinity(precInvScale))
            {
                precs = new double[n];
                for (int i = 0; i < n; i++)
                {
                    precs[i] = precShape;
                }
            }
            else
            {
                precs = Gamma.Sample(rnd, n, precShape, precInvScale);
            }

            // Construct all the mean precision pairs.
            MeanPrecisionPair[] arr = new MeanPrecisionPair[n];

            // Conditionally sample all the mean.
            for (int i = 0; i < n; i++)
            {
                arr[i].Precision = precs[i];
                if (meanScale == 0.0)
                {
                    arr[i].Mean = meanLocation;
                }
                else
                {
                    arr[i].Mean = Normal.Sample(rnd, meanLocation, System.Math.Sqrt(meanScale / precs[i]));
                }
            }

            return arr;
        }*/
    }
}