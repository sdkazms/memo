import os
import ctypes
import subprocess
import time
import pyautogui
import win32gui
import win32api
import tkinter as tk
import xml.etree.ElementTree as ET
# import win32con
# import comtypes.client

class WindowsAPI:
    MOUSEEVENTF_ABSOLUTE = 0x8000
    MOUSEEVENTF_MOVE = 0x0001
    MOUSEEVENTF_LEFTDOWN = 0x0002
    MOUSEEVENTF_LEFTUP = 0x0004
    TOUCH_COORD_TO_PIXEL = 0x0001

    class POINTER_TOUCH_INFO(ctypes.Structure):
        _fields_ = [
            ("pointerInfo", ctypes.c_void_p),
            ("touchFlags", ctypes.c_int),
            ("touchMask", ctypes.c_int),
            ("rcContact", ctypes.c_void_p),
            ("rcContactRaw", ctypes.c_void_p),
            ("orientation", ctypes.c_uint),
            ("pressure", ctypes.c_uint)
        ]

class ExeRunner:
    @staticmethod
    def run_exe(folder_path, exe_name):
        full_path = os.path.join(folder_path, exe_name)
        if os.path.exists(full_path):
            print(f"{exe_name}を実行します...")
            
            try:
                process = subprocess.Popen(full_path)
                time.sleep(5)  # アプリケーションが起動するまで待機
                print(f"{exe_name}が起動しました。")
                return process
            except FileNotFoundError:
                print(f"エラー: {full_path}が見つかりません。")
            except PermissionError:
                print(f"エラー: {full_path}を実行する権限がありません。")
        else:
            print(f"エラー: {full_path}が見つかりません。")

class InputSimulator:
    def __init__(self):
        self.user32 = ctypes.windll.user32

    def simulate_touch(self, x, y):
        self.user32.SetCursorPos(x, y)
        
        # タッチダウン
        self.user32.mouse_event(WindowsAPI.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        time.sleep(0.1)
        
        # タッチアップ
        self.user32.mouse_event(WindowsAPI.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
        
        print(f"タッチシミュレーション: ({x}, {y})")

    def simulate_double_touch(self, x, y):
        self.simulate_touch(x, y)
        time.sleep(0.1)
        self.simulate_touch(x, y)
        print(f"ダブルタッチシミュレーション: ({x}, {y})")

    def mouse_click(self, x, y):
        pyautogui.click(x, y)
        print(f"マウスクリック: ({x}, {y})")

    def mouse_double_click(self, x, y):
        pyautogui.doubleClick(x, y)
        print(f"マウスダブルクリック: ({x}, {y})")

    def keyboard_input(self, text):
        pyautogui.typewrite(text)
        print(f"キーボード入力: {text}")

    # def focus_control_at_position(self, x, y):
    #     """
    #     指定した座標位置にあるコントロールにフォーカスを当てる
    #     """
    #     hwnd = win32gui.WindowFromPoint((x, y))
        
    #     if hwnd:
    #         window_text = win32gui.GetWindowText(hwnd)
    #         class_name = win32gui.GetClassName(hwnd)
            
    #         print(f"座標 ({x}, {y}) にあるウィンドウ:")
    #         print(f"  ウィンドウテキスト: {window_text}")
    #         print(f"  クラス名: {class_name}")
            
    #         # ウィンドウが最小化されている場合は元に戻す
    #         if win32gui.IsIconic(hwnd):
    #             win32gui.ShowWindow(hwnd, win32con.SW_RESTORE)
            
    #         # ウィンドウを表示
    #         win32gui.ShowWindow(hwnd, win32con.SW_SHOW)
            
    #         # 少し待機
    #         time.sleep(0.1)
            
    #         # ウィンドウをアクティブにする
    #         win32gui.SetActiveWindow(hwnd)
            
    #         # フォーカスを設定
    #         win32gui.SetFocus(hwnd)
            
    #         print(f"ウィンドウ '{window_text}' にフォーカスを当てました。")
    #     else:
    #         print(f"座標 ({x}, {y}) にウィンドウが見つかりませんでした。")

class MonitorInfo:
    @staticmethod
    def get_monitor_info():
        monitors = []
        for i, monitor in enumerate(win32api.EnumDisplayMonitors()):
            monitor_info = win32api.GetMonitorInfo(monitor[0])
            monitors.append({
                'index': i,
                'left': monitor_info['Monitor'][0],
                'top': monitor_info['Monitor'][1],
                'width': monitor_info['Monitor'][2] - monitor_info['Monitor'][0],
                'height': monitor_info['Monitor'][3] - monitor_info['Monitor'][1]
            })
        return monitors
    
class MultiMonitorInputSimulator(InputSimulator):

    def click_on_monitor(self, monitor, x, y):
        absolute_x = monitor['left'] + x
        absolute_y = monitor['top'] + y
        self.mouse_click(absolute_x, absolute_y)
        print(f"モニター {monitor['index']} でクリック: ({x}, {y})")

    def double_click_on_monitor(self, monitor, x, y):
        absolute_x = monitor['left'] + x
        absolute_y = monitor['top'] + y
        self.mouse_double_click(absolute_x, absolute_y)
        print(f"モニター {monitor['index']} でダブルクリック: ({x}, {y})")

    def type_on_monitor(self, monitor, x, y, text):
        absolute_x = monitor['left'] + x
        absolute_y = monitor['top'] + y
        self.mouse_click(absolute_x, absolute_y)
        self.keyboard_input(text)
        print(f"モニター {monitor['index']} で入力: '{text}' at ({x}, {y})")

    def simulate_touch_on_monitor(self, monitor, x, y):
        user32 = ctypes.windll.user32
        
        absolute_x = monitor['left'] + x
        absolute_y = monitor['top'] + y
        
        # # タッチ情報の設定
        # touch_info = POINTER_TOUCH_INFO()
        # touch_info.pointerInfo = ctypes.c_void_p()
        # touch_info.touchFlags = 0
        # touch_info.touchMask = 0
        # touch_info.rcContact = ctypes.c_void_p()
        # touch_info.rcContactRaw = ctypes.c_void_p()
        # touch_info.orientation = 90
        # touch_info.pressure = 32000

        # タッチダウン
        user32.SetCursorPos(absolute_x, absolute_y)
        user32.mouse_event(WindowsAPI.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        time.sleep(0.1)

        # タッチアップ
        user32.mouse_event(WindowsAPI.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
        
        print(f"モニター {monitor['index']} でタッチシミュレーション: ({x}, {y})")

    def simulate_double_touch_on_monitor(self, monitor, x, y):
        self.simulate_touch_on_monitor(monitor, x, y)
        time.sleep(0.1)
        self.simulate_touch_on_monitor(monitor, x, y)
        print(f"モニター {monitor['index']} でダブルタッチシミュレーション: ({x}, {y})")

    # def focus_control_on_monitor(self, monitor, x, y):
    #     """
    #     指定したモニター上の座標にあるコントロールにフォーカスを当てる
    #     """
    #     absolute_x = monitor['left'] + x
    #     absolute_y = monitor['top'] + y
        
    #     hwnd = win32gui.WindowFromPoint((absolute_x, absolute_y))
        
    #     if hwnd:
    #         window_text = win32gui.GetWindowText(hwnd)
    #         class_name = win32gui.GetClassName(hwnd)
            
    #         print(f"モニター {monitor['index']} の座標 ({x}, {y}) にあるウィンドウ:")
    #         print(f"  ウィンドウテキスト: {window_text}")
    #         print(f"  クラス名: {class_name}")
          
    #         # win32gui.SetForegroundWindow(hwnd)
    #         # win32gui.SetFocus(hwnd)
                        
    #         # ウィンドウが最小化されている場合は元に戻す
    #         if win32gui.IsIconic(hwnd):
    #             win32gui.ShowWindow(hwnd, win32con.SW_RESTORE)
            
    #         # ウィンドウを表示
    #         win32gui.ShowWindow(hwnd, win32con.SW_SHOW)
            
    #         # 少し待機
    #         time.sleep(0.1)
            
    #         # UIオートメーションを使用してフォーカスを設定
    #         try:
    #             automation = comtypes.client.CreateObject("UIAutomation.UiAutomation", interface=comtypes.gen.UIAutomationClient.IUIAutomation)
    #             element = automation.ElementFromHandle(hwnd)
    #             element.SetFocus()
    #             print(f"ウィンドウ '{window_text}' にフォーカスを当てました。")
    #         except Exception as e:
    #             print(f"フォーカスの設定に失敗しました: {e}")
            
            
    #         # # ウィンドウをアクティブにする
    #         # win32gui.SetActiveWindow(hwnd)
            
    #         # # フォーカスを設定
    #         # win32gui.SetFocus(hwnd)
            
    #         print(f"ウィンドウ '{window_text}' にフォーカスを当てました。")
    #     else:
    #         print(f"モニター {monitor['index']} の座標 ({x}, {y}) にウィンドウが見つかりませんでした。")

class MousePositionTracker:
    @staticmethod
    def calculate_ratio(numerator, denominator):
        return f"{numerator/denominator:.3f}"

    # def click_and_show_position(self, monitor, x, y):
    #     absolute_x = monitor['left'] + x
    #     absolute_y = monitor['top'] + y
    #     pyautogui.click(absolute_x, absolute_y)
    #     time.sleep(0.1)
    #     current_x, current_y = win32gui.GetCursorPos()
    #     relative_x = current_x - monitor['left']
    #     relative_y = current_y - monitor['top']
    #     x_fraction = self.calculate_ratio(relative_x, monitor['width'])
    #     y_fraction = self.calculate_ratio(relative_y, monitor['height'])
    #     print(f"モニター {monitor['index']} でクリック: ({x}, {y})")
    #     print(f"現在のマウス位置: 幅の{x_fraction}, 高さの{y_fraction}")
    
    @staticmethod
    def show_real_time_position(monitor):
        print(f"モニター {monitor['index']} の位置情報をリアルタイムで表示します。")
        print("終了するには Ctrl+C を押してください。")
        try:
            while True:
                current_x, current_y = win32gui.GetCursorPos()
                
                # モニター内での相対位置を計算
                relative_x = current_x - monitor['left']
                relative_y = current_y - monitor['top']
                
                # モニター外の場合はスキップ
                if 0 <= relative_x <= monitor['width'] and 0 <= relative_y <= monitor['height']:
                    
                    # 相対位置を分数で表現
                    x_fraction = MousePositionTracker.calculate_ratio(relative_x, monitor['width'])
                    y_fraction = MousePositionTracker.calculate_ratio(relative_y, monitor['height'])
                    print(f"\r現在のマウス位置: 幅の{x_fraction}, 高さの{y_fraction}", end='', flush=True)
                    
                time.sleep(0.1)
        except KeyboardInterrupt:
            print("\n終了します。")


class XMLHandler:
    # def __init__(self, file_path):
    #     self.file_path = file_path

    # def read_xml(self):
    #     tree = ET.parse(self.file_path)
    #     root = tree.getroot()
        
    #     all_value = []
    #     print("XMLファイルの内容:")
    #     for elem in root:
    #         all_value.append((elem.tag, elem.text))
    #         print(f"{elem.tag}: {elem.text}")
    #     return all_value

    # def write_xml(self, updates):
    #     tree = ET.parse(self.file_path)
    #     root = tree.getroot()
        
    #     for tag, value in updates.items():
    #         elem = root.find(tag)
    #         if elem is not None:
    #             elem.text = value
    #         else:
    #             print(f"警告: {tag}要素が見つかりません。")
                
    #     tree.write(self.file_path, encoding="utf-8", xml_declaration=True)
    #     print("XMLファイルを更新しました。")

    # def write_xml_element(self, tag, value):
    #     tree = ET.parse(self.file_path)
    #     root = tree.getroot()
        
    #     elem = root.find(tag)
    #     if elem is not None:
    #         elem.text = value
    #         tree.write(self.file_path, encoding="utf-8", xml_declaration=True)
    #         print(f"{tag}要素を{value}に更新しました。")
    #     else:
    #         print(f"警告: {tag}要素が見つかりません。")
    #         new_elem = ET.SubElement(root, tag)
    #         new_elem.text = value
    #         tree.write(self.file_path, encoding="utf-8", xml_declaration=True)

    # def read_xml_element(self, tag):
    #     tree = ET.parse(self.file_path)
    #     root = tree.getroot()
        
    #     elem = root.find(tag)
    #     if elem is not None:
    #         print(f"{tag}要素の値: {elem.text}")
    #         return elem.text
    #     else:
    #         print(f"警告: {tag}要素が見つかりません。")
    #         return None
    
    @staticmethod
    def read_xml(file_path):
        tree = ET.parse(file_path)
        root = tree.getroot()
        
        all_value = []
        print("XMLファイルの内容:")
        for elem in root:
            all_value.append((elem.tag, elem.text))
            print(f"{elem.tag}: {elem.text}")
        return all_value

    @staticmethod
    def write_xml(file_path, updates):
        tree = ET.parse(file_path)
        root = tree.getroot()
        
        for tag, value in updates.items():
            elem = root.find(tag)
            if elem is not None:
                elem.text = value
            else:
                print(f"警告: {tag}要素が見つかりません。")
                
        tree.write(file_path, encoding="utf-8", xml_declaration=True)
        print("XMLファイルを更新しました。")

    @staticmethod
    def write_xml_element(file_path, tag, value):
        tree = ET.parse(file_path)
        root = tree.getroot()
        
        elem = root.find(tag)
        if elem is not None:
            elem.text = value
            tree.write(file_path, encoding="utf-8", xml_declaration=True) # TODO: 必要か後で確認する
            print(f"{tag}要素を{value}に更新しました。")
        else:
            print(f"警告: {tag}要素が見つかりません。")
            new_elem = ET.SubElement(root, tag)
            new_elem.text = value

    @staticmethod
    def read_xml_element(file_path, tag):
        tree = ET.parse(file_path)
        root = tree.getroot()
        
        elem = root.find(tag)
        if elem is not None:
            print(f"{tag}要素の値: {elem.text}")
            return elem.text
        else:
            print(f"警告: {tag}要素が見つかりません。")
            return None

class TouchHighlight:
    def __init__(self, monitor):
        self.root = tk.Tk()
        self.root.overrideredirect(True)
        self.root.geometry(f"{monitor['width']}x{monitor['height']}+{monitor['left']}+{monitor['top']}")
        self.root.attributes("-alpha", 0.3)
        self.root.attributes("-topmost", True)
        self.canvas = tk.Canvas(self.root, bg='black', highlightthickness=0)
        self.canvas.pack(fill=tk.BOTH, expand=True)

    def highlight(self, x, y, color='red', radius=20):
        self.canvas.delete("all")
        self.canvas.create_oval(x-radius, y-radius, x+radius, y+radius, fill=color, outline='')
        self.root.update()

    def clear(self):
        self.canvas.delete("all")
        self.root.update()

################################################################################
###テスト用のクラス定義
################################################################################
class MultiMonitorInputTester:
    def __init__(self, multi_monitor_input_simulator):
        self.multi_monitor_input_simulator = multi_monitor_input_simulator
        
    def test_multi_monitor_clicks(self):
        monitors = MonitorInfo.get_monitor_info()
        if len(monitors) < 2:
            print("複数のモニターが検出されませんでした。")
            return
        main_monitor, sub_monitor = monitors[0], monitors[1]
        print("5秒後にクリック操作を開始します...")
        time.sleep(5)
        
        self.multi_monitor_input_simulator.click_on_monitor(main_monitor, main_monitor['width'] // 2, main_monitor['height'] // 2)
        time.sleep(2)
        
        self.multi_monitor_input_simulator.click_on_monitor(sub_monitor, sub_monitor['width'] // 2, sub_monitor['height'] // 2)

    def test_relative_mouse_position(self):
        
        monitors = MonitorInfo.get_monitor_info()

        if len(monitors) < 2:
            print("複数のモニターが検出されませんでした。")
            return

        print("どのモニターの位置情報を表示しますか？")
        print("1: メイン画面")
        print("2: サブ画面")
        
        choice = input("選択してください (1 または 2): ")
        
        if choice == '1':
            selected_monitor = monitors[0]
        elif choice == '2':
            selected_monitor = monitors[1]
        else:
            print("無効な選択です。プログラムを終了します。")
            return

        MousePositionTracker.show_real_time_position(selected_monitor)

    def test_double_click(self):
        # 画面の中央でダブルクリックを実行
        screen_width, screen_height = pyautogui.size()
        center_x = screen_width // 2
        center_y = screen_height // 2
        
        print("5秒後に画面中央でダブルクリックを実行します...")
        time.sleep(5)
        
        self.multi_monitor_input_simulator.mouse_double_click(center_x, center_y)

    def test_touch_actions(self):
        # 画面のサイズを取得
        user32 = ctypes.windll.user32
        screen_width = user32.GetSystemMetrics(0)
        screen_height = user32.GetSystemMetrics(1)
        
        # タッチ位置を計算（例：画面の左上1/4）
        touch_x = screen_width // 4
        touch_y = screen_height // 4
        
        # ダブルタッチ位置を計算（例：画面の右下1/4）
        double_touch_x = screen_width * 3 // 4
        double_touch_y = screen_height * 3 // 4
        
        print("5秒後にタッチとダブルタッチのシミュレーションを開始します...")
        time.sleep(5)
        
        # タッチをシミュレート
        self.multi_monitor_input_simulator.simulate_touch(touch_x, touch_y)
        
        time.sleep(2)  # 操作間の待機時間
        
        # ダブルタッチをシミュレート
        self.multi_monitor_input_simulator.simulate_double_touch(double_touch_x, double_touch_y)

    # def test_focus_control(self):
    #     """
    #     コントロールフォーカスのテスト
    #     """
    #     monitors = MonitorInfo.get_monitor_info()
    #     if len(monitors) < 2:
    #         print("複数のモニターが検出されませんでした。")
    #         return

    #     main_monitor, sub_monitor = monitors[0], monitors[1]

    #     print("5秒後にコントロールフォーカスのテストを開始します...")
    #     time.sleep(5)

    #     # メインモニターの中央にあるコントロールにフォーカス
    #     main_center_x = main_monitor['left'] + main_monitor['width'] // 2
    #     main_center_y = main_monitor['top'] + main_monitor['height'] // 2
    #     self.multi_monitor_input_simulator.focus_control_at_position(main_center_x, main_center_y)
    #     time.sleep(2)

    #     # サブモニターの左上付近にあるコントロールにフォーカス
    #     sub_top_left_x = sub_monitor['left'] + 100
    #     sub_top_left_y = sub_monitor['top'] + 100
    #     self.multi_monitor_input_simulator.focus_control_at_position(sub_top_left_x, sub_top_left_y)

    # def test_focus_control_multi_monitor(self):
    #     """
    #     マルチモニターでのコントロールフォーカスのテスト
    #     """
    #     monitors = MonitorInfo.get_monitor_info()
    #     if len(monitors) < 2:
    #         print("複数のモニターが検出されませんでした。")
    #         return

    #     main_monitor, sub_monitor = monitors[0], monitors[1]

    #     print("5秒後にマルチモニターでのコントロールフォーカスのテストを開始します...")
    #     time.sleep(5)

    #     # メイン画面のテスト
    #     print("\nメイン画面でのテスト:")
    #     # 中央
    #     self.multi_monitor_input_simulator.focus_control_on_monitor(main_monitor, main_monitor['width'] // 2, main_monitor['height'] // 2)
    #     time.sleep(2)
    #     # 左上
    #     self.multi_monitor_input_simulator.focus_control_on_monitor(main_monitor, 100, 100)
    #     time.sleep(2)
    #     # 右下
    #     self.multi_monitor_input_simulator.focus_control_on_monitor(main_monitor, main_monitor['width'] - 100, main_monitor['height'] - 100)
    #     time.sleep(2)

    #     # サブ画面のテスト
    #     print("\nサブ画面でのテスト:")
    #     # 中央
    #     self.multi_monitor_input_simulator.focus_control_on_monitor(sub_monitor, sub_monitor['width'] // 2, sub_monitor['height'] // 2)
    #     time.sleep(2)
    #     # 左上
    #     self.multi_monitor_input_simulator.focus_control_on_monitor(sub_monitor, 100, 100)
    #     time.sleep(2)
    #     # 右下
    #     self.multi_monitor_input_simulator.focus_control_on_monitor(sub_monitor, sub_monitor['width'] - 100, sub_monitor['height'] - 100)

class DesktopAppTester:
    def __init__(self, input_simulator):
        self.input_simulator = input_simulator

    def test_desktop_app(self):
        # アプリケーションを開く（例：メモ帳）
        pyautogui.press('winleft')
        time.sleep(1)
        pyautogui.typewrite('notepad')
        pyautogui.press('enter')
        time.sleep(2)
        
        # テキストを入力
        self.input_simulator.keyboard_input("これは自動テストです。")
        pyautogui.press('enter')
        
        # 特定の位置をクリック（座標は適宜調整）
        self.input_simulator.mouse_click(100, 100)
        
        # アプリケーションを閉じる
        pyautogui.hotkey('alt', 'f4')
        time.sleep(1)
        
        pyautogui.press('n') # 保存せずに閉じる

class TouchHighlightTester:
    def __init__(self, input_simulator):
        self.input_simulator = input_simulator

    def simulate_touch_with_highlight(self, highlight, x, y):
        highlight.highlight(x, y)
        print(f"タッチシミュレーション: ({x}, {y})")
        time.sleep(1)
        highlight.clear()

    def test_touch_highlight(self):
        monitors = MonitorInfo.get_monitor_info()
        if len(monitors) < 2:
            print("複数のモニターが検出されませんでした。")
            return
        
        main_monitor = monitors[0]
        sub_monitor = monitors[1]

        main_highlight = TouchHighlight(main_monitor)
        sub_highlight = TouchHighlight(sub_monitor)
        
        print("5秒後に操作を開始します...")
        time.sleep(5)
        
         # メイン画面での操作
        self.simulate_touch_with_highlight(main_highlight, main_monitor['width'] // 4, main_monitor['height'] // 4)
        time.sleep(1)
        self.simulate_touch_with_highlight(main_highlight, main_monitor['width'] // 2, main_monitor['height'] // 2)
        time.sleep(1)
        
        # サブ画面での操作
        self.simulate_touch_with_highlight(sub_highlight, sub_monitor['width'] // 4, sub_monitor['height'] // 4)
        time.sleep(1)
        self.simulate_touch_with_highlight(sub_highlight, sub_monitor['width'] // 2, sub_monitor['height'] // 2)
        
        main_highlight.root.destroy()
        sub_highlight.root.destroy()


class XMLTester:
    def test_read_write(self):
        file_path = r"C:\git\driver\auto_test/temp_xml.xml"

        # XMLファイルの内容を読み取る
        all_value = XMLHandler.read_xml(file_path)
        
        # 特定の要素の値を読み取る
        XMLHandler.read_xml_element(file_path, "AAAAA2")
        
        # 特定の要素を更新する
        XMLHandler.write_xml_element(file_path, "AAAAA3", "さしすせそ")
        
        # 複数の要素を更新する
        updates = {
            "AAAAA1": "2",
            "AAAAA2": "かきくけこ",
            "BBBBBBB": "0"
        }
        XMLHandler.write_xml(file_path, updates)
        
        # 更新後のXMLファイルの内容を読み取る
        print("\n更新後のXMLファイルの内容:")
        all_value = XMLHandler.read_xml(file_path)
        
        print("\n終了")
        
    # def test_read_write2(self):
        
    #     file_path = r"C:\git\driver\auto_test/temp_xml.xml"

    #     xml_Handler = XMLHandler(file_path)
        
    #     # XMLファイルの内容を読み取る
    #     all_value = xml_Handler.read_xml()
        
    #     # 特定の要素の値を読み取る
    #     xml_Handler.read_xml_element("AAAAA1")
        
    #     # 特定の要素を更新する
    #     xml_Handler.write_xml_element("AAAAA3", "さしすせそ")
        
    #     # 複数の要素を更新する
    #     updates = {
    #         "AAAAA1": "2",
    #         "AAAAA2": "かきくけこ",
    #         "BBBBBBB": "0"
    #     }
    #     xml_Handler.write_xml(updates)
        
    #     # 更新後のXMLファイルの内容を読み取る
    #     print("\n更新後のXMLファイルの内容:")
    #     all_value = xml_Handler.read_xml(file_path)
        
    #     print("\n終了")
        
if __name__ == "__main__":
    input_simulator = MultiMonitorInputSimulator()
    
    multi_monitor_tester = MultiMonitorInputTester(input_simulator)    
    desktop_app_tester = DesktopAppTester(input_simulator)    
    touch_highlight_tester = TouchHighlightTester(input_simulator)
    xml_tester = XMLTester()

    # テスト実行例
    multi_monitor_tester.test_relative_mouse_position()    
    multi_monitor_tester.test_touch_actions()
    multi_monitor_tester.test_double_click()
    multi_monitor_tester.test_multi_monitor_clicks()
    # multi_monitor_tester.test_focus_control_multi_monitor()
    # multi_monitor_tester.test_focus_control()
    
    desktop_app_tester.test_desktop_app()
    touch_highlight_tester.test_touch_highlight()

    xml_tester.test_read_write()
    print('ファイル名:    ', __file__)


