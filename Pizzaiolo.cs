namespace pizzeria
{
     public class Pizzaiolo
    {
        public int _id { get; private set; }
        
        public Pizzaiolo(int id)
        {
            this._id = id;
        }
        
        public void Start()
        {
            Life();
        }
        
        public void Life()
        {
            try
            {
                Console.WriteLine($"Pizzaiolo {_id} waiting for order");
                Program.order_ready.WaitOne();
                
                Thread.Sleep(new Random().Next(50, 200));
                
                PizzaOrder order;
                lock (Program.orderLock)
                {
                    order = Program.order.First();
                    Program.order.RemoveFirst();
                    Console.WriteLine($"Pizzaiolo {_id} took order. Orders left: {Program.order.Count}");
                }
                
                Thread.Sleep(new Random().Next(50, 200));
                order.StartWorking();
                
                Console.WriteLine($"Pizzaiolo {_id} about to finish the pizza slice");
                PizzaSlice slice = order.FinishWorking($"{_id}_slice");
                
                lock (Program.workingSurfaceLock)
                {
                    Program.workingsurface.AddFirst(slice);
                    Console.WriteLine($"Pizzaiolo {_id} added slice. Surface count: {Program.workingsurface.Count}");
                    
                    if (Program.workingsurface.Count == Program.n_slices)
                    {
                        lock (Program.pickUpLock)
                        {
                            Program.pickUp.AddFirst(new PizzaDish(Program.n_slices, $"pizza_{_id}"));
                            Console.WriteLine($"Pizzaiolo {_id} completed pizza. Pizzas in pickup: {Program.pickUp.Count}");
                        }
                        
                        for (int i = 0; i < Program.n_slices; i++)
                        {
                            Program.slice_ready.Release();
                            Console.WriteLine($"Pizzaiolo {_id} released slice {i + 1} semaphore");
                        }
                        
                        Program.workingsurface.Clear();
                        Console.WriteLine($"Pizzaiolo {_id} cleared working surface");
                    }
                }
                
                Console.WriteLine($"Pizzaiolo {_id} finished and terminated");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Pizzaiolo {_id} has an error: {e.Message}");
            }
        }
    }
}

