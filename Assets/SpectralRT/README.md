# Spectral Ray Tracing
This is an initial attempt to ray trace light spectrum. All spectral quantities
(emission spectrum, material properties, imaging filters ...) will be approximated
as the sum of Gaussian functions. Then the Gaussians can be represented by a 3D
vector, where the product of Gaussians is the sum of the vector representations
and the integral of the function can be computed analytically from the terms.
However there are two major problems. First, the sum of Gaussians is not, generally,
a Gaussian this could be solved by keeping an array of 3D vectors representing
the sum of the terms, however this leads into the next problem. Second, assuming
each function is approximated as the sum m Gaussians, then the spectrum of a ray
bouncing off of n surface will have $m^{n+2}$ terms, including the interaction of
the light source and detector filter.

These problems will be addressed by using Monte Carlo sampling of the function.
Each ray will only track a single Gaussian term, the integral of which will be 
accumulated to produce the final approximation.

Note, when approximating nontrivial functions Gaussians with negative amplitudes
will be used and at some points the approximated function may be negative even if
the original function is positive everywhere.

## Gaussian Functions
Each Gaussian term $f(x)$ will be represented as $f(x)=e^{-ax^2 + bx + c + i\pi d}$
where $a, b, c \in \mathbb{R}$, $a \geq 0$ and $d \in \mathbb{Z}$. From this
function we can compute some parameters $x_0$, $f_0$ and $I$, where $x_0 = \frac{b}{2a}$
is the location of the peak, $f_0 = e^{\frac{b^2}{4a} + c + i\pi d}$ is the value
at the peak and $I=\int_{-\infty}^{\infty}f(x)dx = f_0 \sqrt{\frac{\pi}{a}}$ is
the integral of the function. Using these parameters the function has an alternative
form $f(\Delta x) = f_0 e^{-a\Delta x^2}$ with $\Delta x = x - x_0$. Note d is an
integer used to encode the sign of the Gaussian where for positive functions it is
even and for negative functions it is odd.

## Best Fit Gaussian
The method I am using for approximating functions with Gaussians requires that
I find the best fitting Gaussian at a point. For a function $f(x)$ and a target
point $x_0$, first the parameter $d$ is determined from the sign of $f(x_0)$ then
going forward negate $f(x)$ if necessary so that $f(x_0)$ is positive for the
remaining steps. The best fitting Gaussian at $x_0$ can be found calculating the
first three terms of the polynomial approximation of $\ln{f(x)}$.

So defining $x = x_0 + \Delta x$ then
$\ln{f(x_0 + \Delta x)} = c + b(x_0 + \Delta x) - a (x_0 + \Delta x)^2$ ignoring
higher order terms. Repeatedly differentiating and evaluating at $\Delta x = 0$
gives the equations.

$$\ln{f(x_0)} = c + b x_0 - a x_0^2$$

$$\frac{f'(x_0)}{f(x_0)} = b - 2a x_0$$

$$\frac{f(x_0)f''(x_0) - f'(x_0)^2}{f(x_0)^2} = -2a$$

Solving this system of equations gives the coefficients,

$$a = \frac{f'(x_0)^2 - f(x_0)f''(x_0)}{2f(x_0)^2}$$

$$b = x_0 \frac{f'(x_0)^2 - f(x_0)f''(x_0)}{f(x_0)^2} + \frac{f'(x_0)}{f(x_0)}$$

$$c = x_0^2 \frac{f(x_0)f''(x_0) - f'(x_0)^2}{2f(x_0)^2} - x_0 \frac{f'(x_0)}{f(x_0)} + \ln{f(x_0)}$$

Note that if $a < 0$, then the resulting terms do not describe a Gaussian and there
is no best fit Gaussian at that point.
