using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EpSound.MediaControl
{
    public sealed class EsMediaPlayer : MediaTransportControls
    {
        public EsMediaPlayer()
        {
            this.DefaultStyleKey = typeof(EsMediaPlayer);
        }

        public readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(EsMediaPlayer), null);
        public readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(string), typeof(EsMediaPlayer), null);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public string Author
        {
            get => (string)GetValue(AuthorProperty);
            set => SetValue(AuthorProperty, value);
        }
    }
}
