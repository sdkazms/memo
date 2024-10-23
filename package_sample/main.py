from utils import add, substract, multiply, PI # 必要な属性のみをインポート:
from utils import Circle, Rectangle            # 必要な属性のみをインポート:

# from utils import * # 非推奨 全ての公開属性（クラス、関数、変数など）を現在の名前空間にインポート
import utils.math_functions as mf #モジュール全体のimport
import utils.shapes as sh         #モジュール全体のimport

# Circleクラスの使用
circle3 = sh.Circle(1)

# Circle2クラスの使用
circle2 = mf.add(6, 1)

# Circleクラスの使用
circle = Circle(5)
print(f"半径5の円: ")
print(f"Area: {circle.area():.2f}")
print(f"Circumference: {circle.circumference():.2f}")

print("\n" + "="*30 + "\n")

# Rectangleクラスの使用
rectangle = Rectangle(4, 6)
print(f"幅4 高さ6 の長方形")
print(f"Area: {rectangle.area():.2f}")
print(f"Perimeter: {rectangle.perimeter()}")

#関数の使用
result_add = add(5, 3)
result_substract = substract(10, 4)
result_multiply = multiply(2, 6)

print(f"Addition: 5 + 3 = {result_add}")
print(f"Substraction: 10 - 4 = {result_substract}")
print(f"Multiplication: 2 * 6 = {result_multiply}")

# PIの使用
circle_area = PI * (5 ** 2)
print(f"半径5の円の面積: {circle_area}")