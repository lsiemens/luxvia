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

## Gaussian functions
Each Gaussian term $f(x)$ will be represented as $f(x)=e^{-ax^2 + bx + c}$ where
$a, b, c \in \mathbb{R}$ and $a \geq 0$. From this function we can compute some
parameters $x_0$, $f_0$ and $I$, where $x_0 = \frac{b}{2a^2}$ is the location of
the peak, $f_0 = e^{\frac{b^2}{4a^2} + c}$ is the value at the peak and
$I=\int_{-\infty}^{\infty}f(x)dx = \frac{f_0 \sqrt{\pi}}{a}$ is the integral of
the function. Using these parameters the function has an alternative form
$f(\Delta x) = f_0 e^{-a^2\Delta x^2}$ with $\Delta x = x - x_0$.

