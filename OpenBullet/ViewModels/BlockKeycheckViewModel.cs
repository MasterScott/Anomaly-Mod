using RuriLib;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenBullet.ViewModels
{
    public class BlockKeycheckViewModel : ViewModelBase
    {
        private Random rand = new Random(2);

        public BlockKeycheck Block { get; set; }
        public ObservableCollection<KeychainViewModel> KeychainList { get; set; }

        public KeychainViewModel GetKeychainById(int id)
        {
            return KeychainList.Where(x => x.Id == id).First();
        }

        public void RemoveKeychainById(int id)
        {
            KeychainList.Remove(GetKeychainById(id));
        }

        public void AddKeychain()
        {
            KeychainList.Add(new KeychainViewModel(new KeyChain(), rand.Next()));
        }

        public BlockKeycheckViewModel(BlockKeycheck block)
        {
            Block = block;
            KeychainList = new ObservableCollection<KeychainViewModel>();
            foreach (KeyChain keychain in block.KeyChains)
            {
                KeychainList.Add(new KeychainViewModel(keychain, rand.Next()));
            }
        }
    }
}