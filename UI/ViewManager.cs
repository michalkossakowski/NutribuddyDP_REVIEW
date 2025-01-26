namespace NutribuddyDP.UI
{
    internal class ViewManager
    {
        // REVIEW + fajny słownik na różne widoki
        private readonly Dictionary<string, IView> _views = [];

        public void RegisterView(string name, IView view)
        {
            _views[name] = view;
        }

        public void ShowView(string name)
        {
            if (_views.TryGetValue(name, out var view))
            {
                view.Show();
            }
            else
            {
                throw new InvalidOperationException($"View '{name}' not found.");
            }
        }
    }
}
