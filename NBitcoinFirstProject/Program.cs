using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoinFirstProject
{
    class Program
    {
        static void Main(string[] args) {
            //Program.Run(args);
            //TextualTransactionAddress.Run(args);
            //TextualTransactionScript.Run(args);

            //Hasher.Run();

            //FirstTestnetTransaction.Run(args);

            MnemonicKey.test();

            Console.ReadLine();
        }

        public static void Run(string[] args)
        {

            Key privateKey = new Key();
            PubKey publicKey = privateKey.PubKey;
            var publicKeyHash = publicKey.Hash;

            {

                //print del Base58Check
                Console.WriteLine("publicKey:                                 " + publicKey); // es 03a40d9841161f5c20c4b67d353248db81d4591a9f221dc2ec113ea6d7d14bcf89
                Console.WriteLine("publicKey.GetAddress(Network.Main):        " + publicKey.GetAddress(Network.Main)); // es 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
                Console.WriteLine("publicKey.GetAddress(Network.TestNet):     " + publicKey.GetAddress(Network.TestNet)); // es n3zWAo2eBnxLr3ueohXnuAa8mTVBhxmPhq


                BitcoinAddress mainNetAddress = publicKeyHash.GetAddress(Network.Main); //es 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
                BitcoinAddress testNetAddress = publicKeyHash.GetAddress(Network.TestNet); //es n3zWAo2eBnxLr3ueohXnuAa8mTVBhxmPhq

                //print del Base58Check
                Console.WriteLine("publicKeyHash                            : " + publicKeyHash); //c06218cdb28034d0fe481471bd0e0c44c4843d12
                Console.WriteLine("publicKeyHash.GetAddress(Network.Main)   : " + mainNetAddress);
                Console.WriteLine("publicKeyHash.GetAddress(Network.TestNet): " + testNetAddress);

                Console.WriteLine("mainNetAddress.ScriptPubKey:               " + mainNetAddress.ScriptPubKey); //OP_DUP OP_HASH160 c06218cdb28034d0fe481471bd0e0c44c4843d12 OP_EQUALVERIFY OP_CHECKSIG
                Console.WriteLine("testNetAdress.ScriptPubKey:                " + testNetAddress.ScriptPubKey); //OP_DUP OP_HASH160 c06218cdb28034d0fe481471bd0e0c44c4843d12 OP_EQUALVERIFY OP_CHECKSIG

                //note that the ScriptPubKey appears to have nothing to do with the Bitcoin Address, but it does show the 
                // hash of the public key.

                /*
                 Bitcoin Addresses are composed of a version byte which identifies the network where to use the address and 
                  the hash of a public key. 
                  So we can go backwards and generate a bitcoin address from the ScriptPubKey and the network identifier.
                 */
                var paymentScript = publicKeyHash.ScriptPubKey;
                var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(mainNetAddress == sameMainNetAddress); // True

                //It is also possible to retrieve the hash from the ScriptPubKey and generate a Bitcoin Address from it:
                var samePublicKeyHash = (KeyId)paymentScript.GetDestination();
                Console.WriteLine(publicKeyHash == samePublicKeyHash); // True

                var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main);
                Console.WriteLine(mainNetAddress == sameMainNetAddress2); // True

                /*
                Private keys are often represented in Base58Check called a Bitcoin Secret 
                 (also known as Wallet Import Format or simply WIF), like Bitcoin Addresses. 
                */
                BitcoinSecret mainNetPrivateKey = privateKey.GetBitcoinSecret(Network.Main);  // generate our Bitcoin secret(also known as Wallet Import Format or simply WIF) from our private key for the mainnet
                BitcoinSecret testNetPrivateKey = privateKey.GetBitcoinSecret(Network.TestNet);  // generate our Bitcoin secret(also known as Wallet Import Format or simply WIF) from our private key for the testnet
                Console.WriteLine(mainNetPrivateKey); // L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
                Console.WriteLine(testNetPrivateKey); // cVY5auviDh8LmYUW8AfafseD6p6uFoZrP7GjS3rzAerpRKE9Wmuz

                bool WifIsBitcoinSecret = mainNetPrivateKey == privateKey.GetWif(Network.Main);
                Console.WriteLine(WifIsBitcoinSecret); // True

                // *********** section https://programmingblockchain.gitbooks.io/programmingblockchain/content/bitcoin_transfer/transaction.html
                /*
                   query of transactions
                   - https://www.blockchain.com/btc/tx/f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
                   - http://api.qbit.ninja/transactions/f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
                   - or via QBitNinjaClient
                 */
                // Create a client
                QBitNinjaClient client = new QBitNinjaClient(Network.Main);
                // Parse transaction id to NBitcoin.uint256 so the client can eat it
                var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
                // Query the transaction
                GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
                NBitcoin.Transaction transaction = transactionResponse.Transaction;
                Console.WriteLine(transactionResponse.TransactionId); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
                Console.WriteLine(transaction.GetHash()); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94

                //browse its output
                List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
                foreach (var coin in receivedCoins)
                {
                    Money amount = (Money)coin.Amount;

                    Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                    var paymentScript2 = coin.TxOut.ScriptPubKey;
                    Console.WriteLine(paymentScript2);  // It's the ScriptPubKey
                    var address = paymentScript2.GetDestinationAddress(Network.Main);
                    Console.WriteLine(address); // 1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
                    Console.WriteLine();
                }

                //or via transaction.Outputs:
                var outputs = transaction.Outputs;
                foreach (TxOut output in outputs)
                {
                    Money amount = output.Value;

                    Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                    var paymentScript2 = output.ScriptPubKey;
                    Console.WriteLine(paymentScript2);  // It's the ScriptPubKey
                    var address = paymentScript2.GetDestinationAddress(Network.Main);
                    Console.WriteLine(address);
                    Console.WriteLine();
                }

                //now browse its inputs
                var inputs = transaction.Inputs;
                foreach (TxIn input in inputs)
                {
                    OutPoint previousOutpoint = input.PrevOut;
                    Console.WriteLine(previousOutpoint.Hash); // hash of prev tx
                    Console.WriteLine(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
                    Console.WriteLine();
                }

                //As illustration let's create a txout with 21 bitcoin from the first ScriptPubKey in our 
                // current transaction:
                Money twentyOneBtc = new Money(21, MoneyUnit.BTC);
                var scriptPubKey = transaction.Outputs.First().ScriptPubKey;
                TxOut txOut = new TxOut(twentyOneBtc, scriptPubKey);
                //Every TxOut is uniquely addressed at the blockchain level by the ID of the transaction 
                // which include it and its index inside it. We call such reference an Outpoint.

                //For example, the Outpoint of the TxOut with 13.19683492 BTC in our transaction is 
                // (f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94, 0).
                OutPoint firstOutPoint = receivedCoins.First().Outpoint;
                Console.WriteLine(firstOutPoint.Hash); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
                Console.WriteLine(firstOutPoint.N); // 0

                OutPoint firstPreviousOutPoint = transaction.Inputs.First().PrevOut;
                var firstPreviousTransaction = client.GetTransaction(firstPreviousOutPoint.Hash).Result.Transaction;
                Console.WriteLine(firstPreviousTransaction.IsCoinBase); // False

                /*
                 Notice: 0.0002 BTC (or 13.19703492 - 13.19683492) is not accounted for! The difference between 
                  the inputs and outputs are called Transaction Fees or Miner’s Fees. This is the money that the 
                  miner collects for including a given transaction in a block.
                 */
                //fees:
                var fee = transaction.GetFee(transactionResponse.SpentCoins.ToArray());
                Console.WriteLine(fee);
            }

        }
    }
}
