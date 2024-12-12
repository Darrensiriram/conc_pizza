namespace pizzeria
{
   public class Customer
    {
        public int _id { get; private set; }
        
        public Customer(int id)
        {
            this._id = id;
        }
        
        public void Start()
        {
            Life();
        }
        
        public void Life() // customer: feel free to add instructions to make it thread safe.
        {
            try
            {
                Thread.Sleep(new Random().Next(50, 200));
                //wait to order a pizza slice
                //order pizza slice
                Console.WriteLine($"Customer {_id} about to order a pizza slice");

                lock (Program.orderLock)
                {
                    PizzaOrder p = new();
                    Program.order.AddFirst(p);
                    Program.order_ready.Release();
                }
                // wait a bit
                Console.WriteLine($"Customer {_id} waits for a pizza slice");
                Thread.Sleep(new Random().Next(100, 500));

                // only up to 4 people can get a slice from the same pizza
                // pick up pizza slice when possible
                // no more than n_slices slices per pizza so no more than n_slices customers time over the order.

                PizzaDish pizza;
                int pizzaSlices = 0;
                var temp = false;
                Program.slice_ready.WaitOne();

                lock (Program.pickUpLock)
                {
                    if (Program.pickUp.Count > 0)
                    {
                        pizza = Program.pickUp.First();
                        pizzaSlices = pizza.Slices;
                        //remove one slice
                        pizza.RemoveSlice();
                        if (pizza.Slices == 0)
                        {
                            Program.pickUp.RemoveFirst();
                            temp = true;
                        }
                    }
                }
                if (temp)
                {
                    Console.WriteLine($"Customer {_id} has eaten a pizza the final slice total slices: {pizzaSlices} {Program.pickUp.Count}");
                }
                Console.WriteLine($"Customer {_id} has eaten a slice a pizza total slices: {pizzaSlices} {Program.pickUp.Count}");
            }
            catch (Exception e)
            {
                throw new Exception($"Customer {_id} has an error: {e.Message}");
            }
        }
    }
}