public class ToDoListRepository
{
    private static Dictionary<int, ToDoList> lists = new Dictionary<int, ToDoList>();

    public static void AddList(ToDoList list) => lists.Add(list.id, list);
    public static void RemoveList(int id) => lists.Remove(id);
    public static Dictionary<int, ToDoList> GetAllLists() => lists;
    public static List<int> GetIds() => new List<int>(lists.Keys);
    public static bool CheckIfListExists(int id) => lists.ContainsKey(id);

}