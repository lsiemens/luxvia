"""
Approximate sum of gaussians polynomial
"""

from matplotlib import pyplot
import numpy

def gParameters(x_0, f_0, I):
    a = (f_0/I)**2*numpy.pi
    b = 2*a*x_0
    c = numpy.log(numpy.abs(f_0)) - b*x_0 + a*x_0**2
    d = numpy.real(numpy.log(numpy.abs(f_0)/f_0 + 0.0j)/(1.0j*numpy.pi))
    return a, b, c, d

def gaussian(x, a, b, c, d):
    if not numpy.isclose(d%1.0 , 0.0):
        raise RuntimeError(f"d must be an integer: {d}")
    return numpy.real(numpy.exp(-a*x**2 + b*x + c + 1.0j*numpy.pi*d))

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

def find_terms(x, function, index):
    x_0 = x[index]
    f_0 = function[index]
    if (f_0 < 0):
        d = 1
    else:
        d = 0

    fun_p = numpy.gradient(numpy.log(numpy.abs(function)), x)
    fun_p2 = numpy.gradient(fun_p, x)

    a = -0.5*fun_p2[index]
    if a <= 0:
        return None
    b = fun_p[index] + 2*a*x_0
    c = numpy.log(numpy.abs(f_0)) - b*x_0 + a*x_0**2
    d = numpy.real(numpy.log(numpy.abs(f_0)/f_0 + 0.0j)/(1.0j*numpy.pi))
    print(a, b, c, d)
    return a, b, c, d, f_0*numpy.sqrt(numpy.pi/a)

num = 10000
N = 10
x_range = (5, 2000)
useBoundary = False

multiplicationTest = False

x = numpy.linspace(*x_range, num)

function = numpy.empty((N, num))
coefficients = numpy.zeros((N, 4))

block = 0*x
for i in range(5):
    block += 0.8**(i + 1)*lorentz(x, 650 + 100*0.5**i, 25*0.5**i)

#function[0] = plank(x, 5500, 1E16)
#function[0] = lorentz(x, 550, 50)
#function[0] = plank(x, 5500, 1E16)*(1 - 0.5*lorentz(x, 750, 100) - 0.25*lorentz(x, 800, 50))
function[0] = plank(x, 5500, 1E16)*(1 - block)

approximation = 0*x
for i in range(N - 1):
    df = numpy.gradient(function[i], x)
    indices = numpy.where(df[1:]*df[:-1] <= 0)[0]
    if useBoundary:
        indices = indices.tolist() + [1, num - 2]

    best_solver = None
    best_I = 0
    for I in indices:
        solver = find_terms(x, function[i], I)
        if solver is not None:
            if numpy.abs(solver[-1]) > best_I:
                best_solver = solver
                best_I = numpy.abs(solver[-1])

    solver = best_solver

    if solver is None:
        print("error")
        break

    coefficients[i] = numpy.array(solver[:-1])

    gaussian_term = gaussian(x, *coefficients[i])
    approximation += gaussian_term

    function[i + 1] = function[i] - gaussian_term

#    pyplot.plot(x, function[i], label="Original")
#    pyplot.plot(x, approx, label="gauss")
#    pyplot.legend()

#    pyplot.title("PSearch")
#    pyplot.show()

print("Done")
pyplot.plot(x, function[0], label="Original")
pyplot.plot(x, approximation, label="gaussian sum")
pyplot.legend()

dx = x[1] - x[0]
print("Error: ", numpy.sum(numpy.abs(function[0] - approximation))*dx)
pyplot.show()

if multiplicationTest:
    # test multiplication as addition of vectors
    g_coeff = numpy.array(gParameters(1000, -1, -500))
    pyplot.plot(x, gaussian(x, *g_coeff))
    pyplot.show()

    coefficients += g_coeff
    approximation = 0*x
    for i in range(N-1):
        approximation += gaussian(x, *coefficients[i])

    pyplot.plot(x, function[0]*gaussian(x, *g_coeff), label="Original")
    pyplot.plot(x, approximation, label="gaussian sum")
    pyplot.legend()
    pyplot.show()
