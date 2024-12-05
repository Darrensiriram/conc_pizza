namespace pizzeria
{
    public class Customer // you can alter the signature and contents of this class as needed
        // it is not allowed to remove code.
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
            Thread.Sleep(new Random().Next(50, 200));

            //wait to order a pizza slice
            //order pizza slice
            Console.WriteLine($"Customer {_id} about to order a pizza slice");

            PizzaOrder p = new();
            Program.order.AddFirst(p);

            // wait a bit
            Console.WriteLine($"Customer {_id} waits for a pizza slice");

            Thread.Sleep(new Random().Next(100, 500));

            // only up to 4 people can get a slice from the same pizza
            // pick up pizza slice when possible
            // no more than n_slices slices per pizza so no more than n_slices customers time over the order.

            PizzaDish pizza;
            var temp = false;

            pizza = Program.pickUp.First();

            //remove one slice

            pizza.RemoveSlice();

            if (pizza.Slices == 0) //the dish is empty take it out.
            {
                //if it is the last slice remove the pizza from the pick up
                Program.pickUp.RemoveFirst();
                temp = true;
            }

            if (temp)
            {
                Console.WriteLine($"Customer {_id} has eaten a pizza the final slice total slices: {pizza.Slices} {Program.pickUp.Count}");
            }
            Console.WriteLine($"Customer {_id} has eaten a slice a pizza total slices: {pizza.Slices} {Program.pickUp.Count}");
        }
    }
}