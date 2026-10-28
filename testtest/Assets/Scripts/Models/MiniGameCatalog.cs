using System.Collections.Generic;

public static class MiniGameCatalog
{
    public struct GameInfo
    {
        public string sceneName;
        public string displayName;
        public string category;
        public int lectureNumber;
    }

    public static readonly List<GameInfo> Games = new List<GameInfo>
    {
        new GameInfo { sceneName = "MiniGame1",  displayName = "Сортировка событий",         category = "Хронология",    lectureNumber = 1 },
        new GameInfo { sceneName = "MiniGame2",  displayName = "Соедини пары",              category = "Соответствие",  lectureNumber = 1 },
        new GameInfo { sceneName = "MiniGame3",  displayName = "Анализ источников",         category = "Анализ",        lectureNumber = 1 },
        new GameInfo { sceneName = "MiniGame4",  displayName = "Хронологическая лента",     category = "Хронология",    lectureNumber = 2 },
        new GameInfo { sceneName = "MiniGame5",  displayName = "Найди лишнее",              category = "Логика",        lectureNumber = 2 },
        new GameInfo { sceneName = "MiniGame6",  displayName = "Правда или ложь",           category = "Да/Нет",        lectureNumber = 3 },
        new GameInfo { sceneName = "MiniGame7",  displayName = "Генеалогическое древо",     category = "Структура",     lectureNumber = 3 },
        new GameInfo { sceneName = "MiniGame8",  displayName = "Ребусы",                    category = "Головоломка",   lectureNumber = 4 },
        new GameInfo { sceneName = "MiniGame9",  displayName = "Выбор слов",                category = "Лексика",       lectureNumber = 4 },
        new GameInfo { sceneName = "MiniGame10", displayName = "Фото и даты",               category = "Хронология",    lectureNumber = 5 },
        new GameInfo { sceneName = "MiniGame11", displayName = "Кроссворд",                 category = "Головоломка",   lectureNumber = 5 },
        new GameInfo { sceneName = "MiniGame12", displayName = "Газетный редактор",          category = "Творчество",    lectureNumber = 6 },
        new GameInfo { sceneName = "MiniGame13", displayName = "Соотнеси годы",             category = "Хронология",    lectureNumber = 6 },
        new GameInfo { sceneName = "MiniGame14", displayName = "Хронологическая лента 2",   category = "Хронология",    lectureNumber = 7 },
        new GameInfo { sceneName = "MiniGame15", displayName = "Пазл",                      category = "Головоломка",   lectureNumber = 7 },
        new GameInfo { sceneName = "MiniGame16", displayName = "Задание 16",                category = "Разное",        lectureNumber = 8 },
        new GameInfo { sceneName = "MiniGame17", displayName = "Задание 17",                category = "Разное",        lectureNumber = 8 },
        new GameInfo { sceneName = "MiniGame18", displayName = "Мемориал",                  category = "Память",        lectureNumber = 9 },
        new GameInfo { sceneName = "MiniGame19", displayName = "Задание 19",                category = "Разное",        lectureNumber = 9 },
        new GameInfo { sceneName = "MiniGame20", displayName = "Задание 20",                category = "Разное",        lectureNumber = 10 },
        new GameInfo { sceneName = "MiniGame21", displayName = "Историческая викторина",     category = "Викторина",     lectureNumber = 10 },
        new GameInfo { sceneName = "MiniGame22", displayName = "Задание 22",                category = "Разное",        lectureNumber = 11 },
        new GameInfo { sceneName = "MiniGame23", displayName = "Принятие решений",          category = "Стратегия",     lectureNumber = 11 },
        new GameInfo { sceneName = "MiniGame24", displayName = "Задание 24",                category = "Разное",        lectureNumber = 12 },
        new GameInfo { sceneName = "MiniGame25", displayName = "Правовой марафон",          category = "Право",         lectureNumber = 12 },
        new GameInfo { sceneName = "MiniGame26", displayName = "Лестница знаний",           category = "Прогресс",      lectureNumber = 13 },
        new GameInfo { sceneName = "MiniGame27", displayName = "Проверка кандидатов",       category = "Анализ",        lectureNumber = 13 },
        new GameInfo { sceneName = "MiniGame28", displayName = "Диспетчер соответствий",    category = "Соответствие",  lectureNumber = 13 },
        new GameInfo { sceneName = "MiniGame29", displayName = "Конструктор парламента",    category = "Структура",     lectureNumber = 14 },
        new GameInfo { sceneName = "MiniGame30", displayName = "Задание 30",                category = "Разное",        lectureNumber = 14 },
        new GameInfo { sceneName = "MiniGame32", displayName = "Генеалогическое древо 2",   category = "Структура",     lectureNumber = 14 },
        new GameInfo { sceneName = "MiniGame33", displayName = "Газетный редактор 2",       category = "Творчество",    lectureNumber = 14 },
        new GameInfo { sceneName = "MiniGame34", displayName = "Ребусы 2",                  category = "Головоломка",   lectureNumber = 14 },
        new GameInfo { sceneName = "MiniGame35", displayName = "Финальная игра",            category = "Финал",         lectureNumber = 14 },
    };

    public static string[] GetCategories()
    {
        var cats = new HashSet<string>();
        foreach (var g in Games) cats.Add(g.category);
        var arr = new string[cats.Count];
        cats.CopyTo(arr);
        System.Array.Sort(arr);
        return arr;
    }
}
