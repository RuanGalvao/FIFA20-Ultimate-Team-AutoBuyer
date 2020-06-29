using System.Windows.Forms;

namespace FIFA20_Ultimate_Team_AutoBuyer
{
    public class FifaMessageBox
    {
        public static void Show(string content)
        {
            MessageBox.Show(content, Declarations.APPLICATION_NAME);
        }
    }
}
