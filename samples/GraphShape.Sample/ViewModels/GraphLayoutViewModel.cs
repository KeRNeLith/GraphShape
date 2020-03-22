using GraphShape.Utils;

namespace GraphShape.Sample.ViewModels
{
    internal class GraphLayoutViewModel : NotifierObject
    {
        private string _layoutAlgorithmType;

        public string LayoutAlgorithmType
        {
            get => _layoutAlgorithmType;
            set
            {
                if (_layoutAlgorithmType == value)
                    return;

                _layoutAlgorithmType = value;
                OnPropertyChanged();
            }
        }

        private PocGraph _graph;

        public PocGraph Graph
        {
            get => _graph;
            set
            {
                if (_graph == value)
                    return;

                _graph = value;
                OnPropertyChanged();
            }
        }
    }
}