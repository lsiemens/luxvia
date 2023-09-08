# Gaussian Fit
Tests while developing an algorithm to approximate a function as the sum
of Gaussian functions.

The first test `gaussianApprox_polynomial.py`, finds the point with the
largest absolute magnitude of f(x). Then at this point the first three
terms of the Taylor series of log(f(x)) are computed defining the Gaussian
approximation at chat point. This Gaussian approximation is subtracted
from the function and the procedure is repeated. The result is a sequence
of Gaussian approximations each accounting for the highest point of the
previous iteration.

The second test `gaussianApprox_cfit.py`, uses `scipy.optimize.curve_fit`
to find the best fit Gaussian. Note the initial guess is set to an estimate
of the parameters at the highest point. This best fit Gaussian is then
removed from the function and the procedure is repeated. The result is a
sequence of Gaussian sequentially attempting to minimize the error in the
total approximation.

The third test `gaussianApprox.py`, uses a method closely related to the
first test. The difference is that in this test for each extrema of the
function the best fit Gaussian is found at that point. Then, the integral
of the Gaussian approximations are compared for each point, with the point
having the largest integrated contribution being selected. At this point,
like the previous methods this approximation is removed from the function
and the process is repeated. The result is a sequence of Gaussian
approximations each accounting for the largest integrated contribution
from any of the extrema of the function in the previous iteration.

# Initial Observations
Overall the third test preformed the best. This test produced less high
frequency noise than than using `curve_fit` and lower error than the first
method. Additionally it was much more stable than other two and will be
much easier to implement in Unity than attempting to reproduce `curve_fit`.
Note, the reason `curve_fit` tended to produce high frequency noise appears
to be that when fitting the function the best fit curve will tend to have
a lower amplitude then the function at that point while being wider. Given
that this is the case for the region covered by the best fit Gaussian,
then after doing the subtraction you might expect to see a positive bump
in the center with a negative bumps to either side. In contrast fitting a
Gaussian locally, will tend to eliminate the function at the previous peak
but still cause bumps to either side, so in the end creating two rather
than three bumps.
