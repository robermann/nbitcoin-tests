using NBitcoin;
using NBitcoin.DataEncoders;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoinFirstProject
{
    //transaction 930a2114cdaa86e1fac46d15c74e81c09eee1d4150ff9d48e76cb0697d8e1d72
    /*
         Method "Pay-to-Fake-Key-Hash" - P2FKH
The storage afforded by the P2FKH method is 20 bytes per
output, but many outputs can be included in a single transaction. This method has been used
to store text,
9
images (see Fig. 1), and mp3 files
10
in Bitcoin’s blockchain, and is currently the
method employed by tools like Apertus.io.
56

     */
    class TextualTransactionAddress
    {
        public static void Run(string[] args)
        {
            /*
                                query of transactions
                   - https://www.blockchain.com/btc/tx/930a2114cdaa86e1fac46d15c74e81c09eee1d4150ff9d48e76cb0697d8e1d72
                   - http://api.qbit.ninja/transactions/930a2114cdaa86e1fac46d15c74e81c09eee1d4150ff9d48e76cb0697d8e1d72
                     
                   - https://bitcoinsays.com/930a2114cdaa86e1fac46d15c74e81c09eee1d4150ff9d48e76cb0697d8e1d72
             */
            

            // Create a client
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);

            // Parse transaction id to NBitcoin.uint256 so the client can eat it
            uint256 transactionId = uint256.Parse("930a2114cdaa86e1fac46d15c74e81c09eee1d4150ff9d48e76cb0697d8e1d72");

            // Query the transaction
            GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
            NBitcoin.Transaction transaction = transactionResponse.Transaction;

            Console.WriteLine(transactionResponse.TransactionId); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
            Console.WriteLine(transaction.GetHash()); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94

            printAddressesAsAcii(transactionResponse.ReceivedCoins);

        }

        private static void printAddressesAsAcii(List<ICoin> coins)
        {
            Console.WriteLine("**** Addresses:");
           

            foreach (var coin in coins)
            {
                Script paymentScript = coin.TxOut.ScriptPubKey;
                BitcoinAddress address = paymentScript.GetDestinationAddress(Network.Main);

                if (address!= null) {
                    byte[] ascii = Encoders.Base58.DecodeData(address.ToString());

                    var str = System.Text.Encoding.Default.GetString(ascii);

                    Console.WriteLine(str);
                }


            }
        }

        private static void printAddresses(List<ICoin> coins)
        {
            Console.WriteLine("**** Addresses:");

            foreach (var coin in coins)
            {
                Script paymentScript = coin.TxOut.ScriptPubKey;
                var address = paymentScript.GetDestinationAddress(Network.Main);

                Console.WriteLine(address);
            }
        }

        private static void print(List<ICoin> coins)
        {
            foreach (var coin in coins)
            {
                //in Satoshi, as in the transaction
                Money amount = (Money)coin.Amount;
                //Console.WriteLine(amount == coin.TxOut.Value); //True

                Console.WriteLine("Amount (BTC): " + amount.ToDecimal(MoneyUnit.BTC)); // 1 BTC (unspendable!)

                Script paymentScript = coin.TxOut.ScriptPubKey;
                Console.WriteLine("ScriptPubKey: " + paymentScript);  // ScriptPubKey: OP_DUP OP_HASH160 20ea5dac7986bd8104721a74cbda718ccb6e29f4 OP_EQUALVERIFY OP_CHECKSIG

                String hexScript = paymentScript.ToHex();
                Console.WriteLine("hexScript: " + hexScript); //76a91420ea5dac7986bd8104721a74cbda718ccb6e29f488ac

                IEnumerable<Op> ops = paymentScript.ToOps();

                //Console.WriteLine("******* ops:");
                foreach (Op op in ops)
                {
                    //Console.WriteLine("op: " + op.Name + " ");
                    byte[] bytes = op.ToBytes();
                    byte[] data = op.PushData;
                    if (data != null)
                    {
                        Console.Write("contains "+data.Length + " bytes"); //20 bytes for OP_HASH160
                    }
                    Console.WriteLine();

                }



                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address); // 1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
                //Console.WriteLine();
            }
        }
    }
}
