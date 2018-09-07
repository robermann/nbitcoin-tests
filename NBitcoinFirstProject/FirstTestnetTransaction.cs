using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.RPC;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBitcoinFirstProject
{
    class FirstTestnetTransaction
    {
        public static void Run(string[] args)
        {
            //Transaction1();
            //Transaction2();
            //Transaction3();
            Transaction4();
        }

        //transaction https://live.blockcypher.com/btc-testnet/tx/4761929185a3dd80f23670efb854ba3d7ab5a151f296ea7c38015ceb821ee838/
        public static void Transaction1()
        {
            ////P2WPKH - PS2SH = (Segwit P2PKH wrapped into P2SH for interoperability)
            //https://bitcoin.stackexchange.com/questions/59231/how-to-sign-a-segwit-transaction-via-nbitcoin

            var bitcoinPrivateKey = new BitcoinSecret("cS88F3sJycinEUfGGMQH4w4izaNjEec97VAHsoQZYmDjZJwK1wuf");
            var key = bitcoinPrivateKey.PrivateKey;

            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();

            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("8722a9a073fc4d7d27cf07540ddee2d4449f36d1470eddedb8bd8a70ded88fe3");
            

            var transactionResponse = client.GetTransaction(transactionId).Result;
            Coin[] coinsToSpend = transactionResponse.Transaction.Outputs.AsCoins().ToArray();
            var destinationAddress = BitcoinAddress.Create("2NA5L1cYrMRQEUYisWCK7bjfM1UFdxumYps", network);

            //coinsToSpend = new  Coin[]{coinsToSpend[83] };

            var coins =
                coinsToSpend
                .Where(c => c.TxOut.ScriptPubKey == bitcoinPrivateKey.PubKey.WitHash.ScriptPubKey.PaymentScript)
                .ToArray();

            //expect just 1 coin, coinsToSpend[83]

            TransactionBuilder builder = new TransactionBuilder();
            builder.AddCoins(coins); 
            builder.AddKeys(bitcoinPrivateKey);
            builder.Send(destinationAddress, new Money(0.000000015m, MoneyUnit.BTC));
            builder.SendFees(Money.Coins(0.001m));
            builder.SetChange(address.ScriptPubKey);
            var signedTx = builder.BuildTransaction(true);
            String hexPreVerify = signedTx.ToHex();
            Boolean b = builder.Verify(signedTx);
            String hex = signedTx.ToHex(); //"01000000000101e38fd8de708abdb8eddd0e47d1369f44d4e2de0d5407cf277d4dfc73a0a922875300000017160014a017d15c9cfacea59c4a3984b32f2282e617de8effffffff01f6b4b600000000001976a914a017d15c9cfacea59c4a3984b32f2282e617de8e88ac02473044022028f8440e4d85eca57a99fadda133fc9ce48b523a4b4d2438aaaad0a296d8d7270220112eb54f7e8a1f311b6b4dc0a15c4b3a5311948724a8aa0657da52da6b26fc68012102b6d3f00966c6797c69b98cc7d4f7ec0e9ec8aee68f2fccb5cec061eeaee47f8900000000"

            BroadcastResponse broadcastResponse = client.Broadcast(signedTx).Result;

            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(signedTx.GetHash());
            }
        }

        //transaction https://live.blockcypher.com/btc-testnet/tx/63ea3b0cac9e9afc188b42785a63f5bc84dd120400a5e18eccbbbb82adeadf2c/
        private static void Transaction2()
        {
            var bitcoinPrivateKey = new BitcoinSecret("cS88F3sJycinEUfGGMQH4w4izaNjEec97VAHsoQZYmDjZJwK1wuf");
            var key = bitcoinPrivateKey.PrivateKey;

            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();

            var client = new QBitNinjaClient(network);
            
            var transactionId = uint256.Parse("4761929185a3dd80f23670efb854ba3d7ab5a151f296ea7c38015ceb821ee838");

            var transactionResponse = client.GetTransaction(transactionId).Result;

            Console.WriteLine(transactionResponse.TransactionId);

            if (transactionResponse.Block != null)
            {
                Console.WriteLine(transactionResponse.Block.Confirmations);
            }
            

            #region  detect the correct outpount to use

            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                    //amount: {0.11973878 }
                }
            }
            if (outPointToSpend == null)
                throw new Exception("TxOut doesn't contain our WitHash.ScriptPubKey.PaymentScript");

            Console.WriteLine("We want to spend # {0} outpoint", outPointToSpend.N + 1);
            #endregion 


            #region How much you want to spend

            // How much you want to spend
            var destinationAmount = new Money(0.00036930m, MoneyUnit.BTC);

            // How much miner fee you want to pay
            var minerFee = new Money(0.00005000m, MoneyUnit.BTC);  //0.00005 BTC

            // How much you want to get back as change
            var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
            var changeAmount = txInAmount - destinationAmount - minerFee;

            #endregion

            #region create TxOuts
            var destinationAddress = BitcoinAddress.Create("2NA5L1cYrMRQEUYisWCK7bjfM1UFdxumYps", network);

            TxOut destinationTxOut = new TxOut()
            {
                Value = destinationAmount,
                ScriptPubKey = destinationAddress.ScriptPubKey
            };

            TxOut changeTxOut = new TxOut()
            {
                Value = changeAmount,
                ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            };

            var message = "Sound me on www.codesounding.org!";
            var bytes = Encoding.UTF8.GetBytes(message);
            TxOut msgTxtOut = new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            };

            #endregion

            #region build transaction
            Transaction transaction = Transaction.Create(network);
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });
            transaction.Outputs.Add(destinationTxOut);
            transaction.Outputs.Add(changeTxOut);
            transaction.Outputs.Add(msgTxtOut);

            //see https://live.blockcypher.com/btc/decodetx/
            String txHex = transaction.ToHex(); 
            #endregion

            //transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.PubKey.WitHash.ScriptPubKey.PaymentScript;
            transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
            
            #region sign transaction
            transaction.Sign(bitcoinPrivateKey, false);
            String txHexPostSign = transaction.ToHex(); 
            #endregion

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                var txId = transaction.GetHash();
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(txId);
            }
        }

        //transaction https://live.blockcypher.com/btc-testnet/tx/ca678d531e9c8a3cd95e1e33a2562cf0b8a44793e51b46ac7602a72cbe7e1bd0/
        public static void Transaction3()
        {
            QBitNinjaClient client;

            Transaction signedTx = getTransaction(out client);

            BroadcastResponse broadcastResponse = client.Broadcast(signedTx).Result;

            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(signedTx.GetHash()); //id: ca678d531e9c8a3cd95e1e33a2562cf0b8a44793e51b46ac7602a72cbe7e1bd0
            }
        }

        //fail double-spend
        public static void Transaction4()
        {
            QBitNinjaClient client;

            Transaction transaction = getTransaction(out client);

            try
            {
                RPCClient rc = new RPCClient(Network.TestNet);
                rc.SendRawTransaction(transaction);
            }
            catch (Exception e)
            {
                String msg = e.Message;

                Console.WriteLine("error: " + msg); //error code: -27, msg: transaction already in block chain
            }
            
            
        }

        private static Transaction getTransaction(out QBitNinjaClient client)
        {
            var bitcoinPrivateKey = new BitcoinSecret("cS88F3sJycinEUfGGMQH4w4izaNjEec97VAHsoQZYmDjZJwK1wuf");
            var key = bitcoinPrivateKey.PrivateKey;

            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();

            client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("63ea3b0cac9e9afc188b42785a63f5bc84dd120400a5e18eccbbbb82adeadf2c");


            var transactionResponse = client.GetTransaction(transactionId).Result;
            Coin[] coinsToSpend = transactionResponse.Transaction.Outputs.AsCoins().ToArray();
            var destinationAddress = BitcoinAddress.Create("2NFdSeH15r3ua6x1pV8zVg18pwBz8Rxy285", network);


            var coins =
                coinsToSpend
                .Where(c => c.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                .ToArray();

            TransactionBuilder builder = new TransactionBuilder();
            builder.AddCoins(coins);
            builder.AddKeys(bitcoinPrivateKey);
            builder.Send(destinationAddress, new Money(0.02m, MoneyUnit.BTC));
            builder.SendFees(Money.Coins(0.001m));
            builder.SetChange(address.ScriptPubKey);
            Transaction signedTx = builder.BuildTransaction(true);
            String hexPreVerify = signedTx.ToHex();

            NBitcoin.Policy.TransactionPolicyError[] errors = null;
            Boolean b = builder.Verify(signedTx, out errors);
            if (errors != null)
            {
                foreach (var e in errors)
                {
                    Console.WriteLine("builder error-> " + e.ToString());
                }
            }
            String hex = signedTx.ToHex(); //01000000012cdfeaad82bbbbcc8ee1a5000412dd84bcf5635a78428b18fc9a9eac0c3bea63010000006b48304502210088bd03eec8b7b4ac84e76724354b139da50a8f7028a29a0b2eef8071965a9d7c02205ddbd388bab940dfe18bb4863983473b0dbdc475048b8216560931cf2e46e1ae012102b6d3f00966c6797c69b98cc7d4f7ec0e9ec8aee68f2fccb5cec061eeaee47f89ffffffff020c069600000000001976a914a017d15c9cfacea59c4a3984b32f2282e617de8e88ac80841e000000000017a914f5867b7847aad72d7f74675391a5945c5c43268a8700000000

            return signedTx;
        }
    }
}
