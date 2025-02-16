import os
import winreg
import requests
import subprocess
import webbrowser

download_url = 'http://drive.google.com/uc?export=download&id=18Yr6wfSAJZTqhttMFVDNx7pZkez2vJBq'
game_name = 'Goose Goose Duck'

def find_game_path():
    """Ищет путь к игре через реестр"""
    # Открываем ключ реестра
    key_path = r'Software\Classes\gaggleggd\shell\open\command'
    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path) as key:
            try:
                string_path = winreg.QueryValueEx(key, '')[0] # Получаем строку, содержащую путь исполняемого файла
                file_path = string_path.replace('"', '') # Убираем " в строке
                directory = os.path.dirname(file_path)
                print(f'Найден путь: {directory}')
                return directory
            except FileNotFoundError:
                print("Значение не найдено")
    except WindowsError as e:
        print(f"Ошибка при доступе к реестру: {e}")
    return None

def download_file(url, file_path):
    """Загружает файл по ссылке и сохраняет его в папке игры"""
    try:
        response = requests.get(url) # Отправляет запрос GET
        response.raise_for_status() # Проверка статуса ответа на запрос
        with open(file_path, 'wb') as file:
            file.write(response.content)
        print("Файл успешно загружен")
    except requests.RequestException as e:
        print(f"Не удалось загрузить файл: {e}")

def set_registry_values(reg_file_path):
    """Устанавливает значения в реестре из загруженного файла .reg"""
    try:
        subprocess.run(['regedit', '/s', reg_file_path], check=True)
        print(f"Успешно записано в реестр")
    except subprocess.CalledProcessError as e:
        print(f"Ошибка: {e}")


def find_steam_id(game_name):
    """Ищет SteamID игры в реестре"""
    # Открываем ключ реестра
    app_key_path = r"SOFTWARE\Valve\Steam\Apps"
    try:
            with winreg.OpenKey(winreg.HKEY_CURRENT_USER, app_key_path) as app_key:
                for idx in range(winreg.QueryInfoKey(app_key)[0]):
                    app_id = winreg.EnumKey(app_key, idx) # Получаем имя подключа
                    subkey = winreg.OpenKey(app_key, app_id) # Открываем подключ для поиска названия игры
                    try:
                        # Читаем значение "name" для игры
                        name = winreg.QueryValueEx(subkey, "name")[0]
                        if game_name.lower() in name.lower():  # Проверяем, совпадает ли имя
                            return app_id  # Возвращаем AppID
                    except FileNotFoundError:
                        pass

    except WindowsError as e:
        print(f"Ошибка при доступе к реестру: {e}")

    return None

def launch_game(steam_id):
    """Запускает игру через Steam"""
    # Формируем URL для запуска игры
    steam_url = f"steam://run/{steam_id}"
    # Открываем URL
    webbrowser.open(steam_url)

def main():
    download_path = find_game_path()
    file_path = os.path.join(download_path, 'goose-offvolume.reg')
    download_file(download_url, file_path)

    set_registry_values(file_path)

    steam_id = find_steam_id(game_name)
    if steam_id:
        print(f"Запускаем игру: {game_name} (SteamID: {steam_id})")
        launch_game(steam_id)

if __name__ == "__main__":
    main()
