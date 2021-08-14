using Sandbox;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShop : TTTPanel
    {
        public static QuickShop Instance;

        private static ShopItemData? _selectedItemData;

        private Header _header;
        private Footer _footer;
        private readonly Content _content;

        private int _credits = 0;

        public QuickShop()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/quickshop/QuickShop.scss");

            _header = new(this);
            _content = new(this);
            _footer = new(this);

            IsShowing = false;
        }

        public void Update()
        {
            _content.Update();
        }

        public override void Tick()
        {
            base.Tick();

            if (!IsShowing)
            {
                return;
            }

            int credits = (Local.Pawn as TTTPlayer).Credits;

            if (_credits != credits)
            {
                _credits = credits;

                Update();
            }
        }

        public bool CheckAccess()
        {
            return (Local.Pawn as TTTPlayer).Role.CanBuy();
        }
    }
}

namespace TTTReborn.Player
{
    using UI;

    public partial class TTTPlayer
    {
        [ClientCmd("+ttt_quickshop")]
        public static void OpenQuickshop()
        {
            if (QuickShop.Instance == null || !QuickShop.Instance.CheckAccess())
            {
                return;
            }

            QuickShop.Instance.Update();

            QuickShop.Instance.IsShowing = true;
        }

        [ClientCmd("-ttt_quickshop")]
        public static void CloseQuickshop()
        {
            if (QuickShop.Instance == null)
            {
                return;
            }

            QuickShop.Instance.IsShowing = false;
        }
    }
}
