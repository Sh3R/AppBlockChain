using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using blockChainWebApp.API.Models;

namespace blockChainWebApp.API.DAL
{
    public class BlockRepository
    {
        private readonly string _connectionString;

        public BlockRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AzureSQLConnection"].ConnectionString;
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IEnumerable<Block> GetAllBlocks()
        {
            var blocks = new List<Block>();
            using (var connection = GetConnection())
            {
                var command = new SqlCommand("SELECT * FROM Blocks", connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var block = new Block
                    {
                        Hash = reader["Hash"].ToString(),
                        PreviousHash = reader["PreviousHash"].ToString(),
                        Signature = reader["Signature"].ToString(),
                        Timestamp = Convert.ToInt64(reader["Timestamp"] != DBNull.Value ? reader["Timestamp"] : 0),
                        ReceiptPositions = reader["BlockData"].ToString(),
                    };
                    blocks.Add(block);
                }
                connection.Close();
            }

            return blocks;
        }

        //To Add new block record    
        public void AddBlock(Block block)
        {
            using (var con = GetConnection())
            {
                var cmd = new SqlCommand(
                    "INSERT INTO Blocks(Hash,PreviousHash,Signature,BlockData,Timestamp) " +
                    "VALUES (@Hash,@PreviousHash,@Signature,@BlockData,@TimeStamp)",
                    con);

                cmd.Parameters.AddWithValue("@Hash", block.Hash);
                cmd.Parameters.AddWithValue("@PreviousHash", block.PreviousHash);
                cmd.Parameters.AddWithValue("@Timestamp", block.Timestamp);
                cmd.Parameters.AddWithValue("@Signature", block.Signature);
                cmd.Parameters.AddWithValue("@BlockData", block.ReceiptPositions);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}