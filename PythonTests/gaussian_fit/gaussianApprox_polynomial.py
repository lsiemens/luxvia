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

for i in range(N - 1):
    index = numpy.argmax(numpy.abs(fun[i]))
    x_0 = x[index]
    f_0 = fun[i, index]
    if (f_0 < 0):
        d.append(-1)
    else:
        d.append(1)

    fun_p = numpy.gradient(numpy.log(d[i]*fun[i]), x)
    fun_p2 = numpy.gradient(fun_p, x)

    a.append(-0.5*fun_p2[index])
    b.append(fun_p[index] + 2*a[i]*x_0)
    c.append(numpy.log(d[i]*fun[i, index]) - b[i]*x_0 + a[i]*x_0**2)
    approx = gaussian(x, a[i], b[i], c[i], d[i])

    fun[i + 1] = fun[i] - approx
#    print(f"Solution[a:{a[i]}, b:{b[i]}, c:{c[i]}]")

#    pyplot.plot(x, fun[i], label="Original")
#    pyplot.plot(x, approx, label="gauss")
#    pyplot.legend()

#    pyplot.title("polynomial")
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


