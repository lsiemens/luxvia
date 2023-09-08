"""
Approximate sum of gaussians polynomial
"""

from matplotlib import pyplot
import numpy

def gaussian(x, a, b, c, d=1):
    return d*numpy.exp(-a*x**2 + b*x + c)

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
c = []
d = []

def find_terms(x, func, index):
    x_0 = x[index]
    f_0 = func[index]
    if (f_0 < 0):
        d = -1
    else:
        d = 1

    fun_p = numpy.gradient(numpy.log(d*func), x)
    fun_p2 = numpy.gradient(fun_p, x)

    a = -0.5*fun_p2[index]
    if a <= 0:
        return None
    b = fun_p[index] + 2*a*x_0
    c = numpy.log(d*f_0) - b*x_0 + a*x_0**2
    return a, b, c, d, d*f_0*numpy.sqrt(numpy.pi/a)

for i in range(N - 1):
    df = numpy.gradient(fun[i], x)
    indices = numpy.where(df[1:]*df[:-1] <= 0)[0]
    print(indices)
    print(x[indices])

    best_solver = None
    best_I = 0
    for I in indices:
        solver = find_terms(x, fun[i], I)
        if solver is not None:
            if solver[-1] > best_I:
                best_solver = solver
                best_I = solver[-1]

    solver = best_solver

    if solver is None:
        print("error")
        break
    _a, _b, _c, _d, _I = solver
    a.append(_a)
    b.append(_b)
    c.append(_c)
    d.append(_d)

    approx = gaussian(x, a[i], b[i], c[i], d[i])

    fun[i + 1] = fun[i] - approx
#    print(f"Solution[a:{a[i]}, b:{b[i]}, c:{c[i]}]")

#    pyplot.plot(x, fun[i], label="Original")
#    pyplot.plot(x, approx, label="gauss")
#    pyplot.legend()

#    pyplot.title("PSearch")
#    pyplot.show()

print("Done")
pyplot.plot(x, fun[0], label="Original")
final = 0*x
for i in range(N-1):
    final += gaussian(x, a[i], b[i], c[i], d[i])
pyplot.plot(x, final, label="gaussian sum")
pyplot.legend()

dx = x[1] - x[0]
print("Error: ", numpy.sum(numpy.abs(fun[0] - final))*dx)
pyplot.show()


