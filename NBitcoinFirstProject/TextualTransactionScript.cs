﻿using NBitcoin;
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
    //transaction d29c9c0e8e4d2a9790922af73f0b8d51f0bd4bb19940d9cf910ead8fbe85bc9b

    /*
     * Method "OP_RETURN".
     */
    class TextualTransactionScript
    {
        public static void Run(string[] args)
        {

            /*
            String hex = "6a4dd7035765277265206e6f20737472616e6765727320746f206c6f76650a596f75206b6e6f77207468652072756c657320616e6420736f20646f20490a412066756c6c20636f6d6d69746d656e74277320776861742049276d207468696e6b696e67206f660a596f7520776f756c646e27742067657420746869732066726f6d20616e79206f74686572206775790a49206a7573742077616e6e612074656c6c20796f7520686f772049276d206665656c696e670a476f747461206d616b6520796f7520756e6465727374616e640a0a43484f5255530a4e6576657220676f6e6e61206769766520796f752075702c0a4e6576657220676f6e6e61206c657420796f7520646f776e0a4e6576657220676f6e6e612072756e2061726f756e6420616e642064657365727420796f750a4e6576657220676f6e6e61206d616b6520796f75206372792c0a4e6576657220676f6e6e612073617920676f6f646279650a4e6576657220676f6e6e612074656c6c2061206c696520616e64206875727420796f750a0a5765277665206b6e6f776e2065616368206f7468657220666f7220736f206c6f6e670a596f75722068656172742773206265656e20616368696e672062757420796f7527726520746f6f2073687920746f207361792069740a496e7369646520776520626f7468206b6e6f7720776861742773206265656e20676f696e67206f6e0a5765206b6e6f77207468652067616d6520616e6420776527726520676f6e6e6120706c61792069740a416e6420696620796f752061736b206d6520686f772049276d206665656c696e670a446f6e27742074656c6c206d6520796f7527726520746f6f20626c696e6420746f20736565202843484f525553290a0a43484f52555343484f5255530a284f6f68206769766520796f75207570290a284f6f68206769766520796f75207570290a284f6f6829206e6576657220676f6e6e6120676976652c206e6576657220676f6e6e6120676976650a286769766520796f75207570290a284f6f6829206e6576657220676f6e6e6120676976652c206e6576657220676f6e6e6120676976650a286769766520796f75207570290a0a5765277665206b6e6f776e2065616368206f7468657220666f7220736f206c6f6e670a596f75722068656172742773206265656e20616368696e672062757420796f7527726520746f6f2073687920746f207361792069740a496e7369646520776520626f7468206b6e6f7720776861742773206265656e20676f696e67206f6e0a5765206b6e6f77207468652067616d6520616e6420776527726520676f6e6e6120706c61792069742028544f2046524f4e54290a0a";
            byte[] ascii = Encoders.Hex.DecodeData(hex);
            String str = System.Text.Encoding.Default.GetString(ascii);

            Console.WriteLine(str);
            */
            /*
                                query of transactions
                   - https://www.blockchain.com/btc/tx/d29c9c0e8e4d2a9790922af73f0b8d51f0bd4bb19940d9cf910ead8fbe85bc9b
                   - http://api.qbit.ninja/transactions/d29c9c0e8e4d2a9790922af73f0b8d51f0bd4bb19940d9cf910ead8fbe85bc9b
                     
                   - https://bitcoinsays.com/d29c9c0e8e4d2a9790922af73f0b8d51f0bd4bb19940d9cf910ead8fbe85bc9b
             */


            // Create a client
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);

            // Parse transaction id to NBitcoin.uint256 so the client can eat it
            uint256 transactionId = uint256.Parse("d29c9c0e8e4d2a9790922af73f0b8d51f0bd4bb19940d9cf910ead8fbe85bc9b");

            // Query the transaction
            GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
            NBitcoin.Transaction transaction = transactionResponse.Transaction;

            printScriptsAsAcii(transactionResponse.ReceivedCoins);

        }

        private static void printScriptsAsAcii(List<ICoin> coins)
        {
            Console.WriteLine("**** Scripts:");
           

            foreach (var coin in coins)
            {

                byte[] byteScript = coin.TxOut.ScriptPubKey.ToBytes();

                /*
                 Because it includes 3 "command" bytes (4d, d7, 03)
                 
                         6a             4d                    d7 03 
                     OP_RETURN    OP_PUSHDATA2    (03d7 = 983 following bytes)

                By the way, see my question about the max allowed size:
                   https://bitcoin.stackexchange.com/questions/78572/op-return-max-bytes-clarification
                 */

                //remove first 4 bytes
                byteScript = byteScript.Skip(4).ToArray();

                //get the ASCII
                String str = System.Text.Encoding.Default.GetString(byteScript);
                Console.WriteLine(str);

             }

        }
    }
}
