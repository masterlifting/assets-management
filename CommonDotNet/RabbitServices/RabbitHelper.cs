﻿
using CommonServices.RabbitServices.Configuration;

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CommonServices.RabbitServices
{
    public static class RabbitHelper
    {
        public static bool TrySerialize<T>(string data, out T? entity) where T : class
        {
            entity = null;

            try
            {
                entity = JsonSerializer.Deserialize<T>(data);
                return true;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Serializable exception: " + ex.Message);
                return false;
            }
        }
    }

    internal class QueueComparer : IEqualityComparer<Queue>
    {
        public bool Equals(Queue? x, Queue? y) => x!.NameEnum == y!.NameEnum;
        public int GetHashCode(Queue obj) => obj.NameString.GetHashCode();
    }
}
