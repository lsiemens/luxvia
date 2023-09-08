"""
Approximate sum of gaussians using curve_fit
"""

from matplotlib import pyplot
from scipy.optimize import curve_fit
import numpy

def gaussian(x, a, b, d):
    return d*numpy.exp(-a*(x - b)**2)

def plank(x, T, mult=1):
    """
    x in nm
    T in K
    """
    hc2 = 5.955E1
    hc_k = 1.439E7
    return mult*(2*hc2/(x**5))/(numpy.exp(hc_k/(x*T)) - 1)

def lorentz(x, x_0, width=1, mult=1):
    return mult/(1 + (2*(x - x_0)/width)**2)

num = 10000
N = 2
x_range = (5, 2000)

x = numpy.linspace(*x_range, num)

fun = numpy.empty((N, num))

block = 0*x
for i in range(5):
    block += 0.8**(i + 1)*lorentz(x, 650 + 100*0.5**i, 25*0.5**i)

#fun[0] = plank(x, 5500, 1E16)
#fun[0] = lorentz(x, 550, 50)
#fun[0] = plank(x, 5500, 1E16)*(1 - 0.5*lorentz(x, 750, 100) - 0.25*lorentz(x, 800, 50))
fun[0] = plank(x, 5500, 1E16)*(1 - block)

a = []
b = []
d = []

done = 0*x
for i in range(N - 1):
    index = numpy.argmax(numpy.abs(fun[i]))
    dx = numpy.mean(x[1:] - x[:-1])
    x_0 = x[index]
    f_0 = fun[i, index]
    I = numpy.sum(numpy.abs(fun[i]))*dx
    width = numpy.pi*(f_0/I)**2
    try:
        params, covariance = curve_fit(gaussian, x, fun[i], p0=[width, x_0, f_0])
    except RuntimeError as e:
        params, covariance = curve_fit(gaussian, x, fun[i], p0=[width, x_0, -f_0])

    a.append(params[0])
    b.append(params[1])
    d.append(params[2])
    app = gaussian(x, a[i], b[i], d[i])
    done += app
    fun[i + 1] = fun[i] - app

#    pyplot.plot(x, fun[i], label="data")
#    pyplot.plot(x, app, label="app")
#    pyplot.legend()
#    pyplot.title("curve_fit")
#    pyplot.show()

print("DONE!!!")
pyplot.plot(x, fun[0], label="data")
pyplot.plot(x, done, label="app")
pyplot.legend()

dx = x[1] - x[0]
print("Error: ", numpy.sum(numpy.abs(fun[0] - done))*dx)
pyplot.show()

