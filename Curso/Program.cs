using System;
using System.Collections.Generic;
using System.Linq;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //InserirDados();
            //InserirDadosEmMassa();
            //ConsultarDados();
            //CadastraPedido();
            //AtualizarDados();
            RemoverRegistro();
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new Data.ApplicationContext();

            //db.Produtos.Add(produto); //Recomendado
            //db.Set<Produto>().Add(produto); //Recomendado
            //db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros = db.SaveChanges();

            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            // var cliente = new Cliente
            // {
            //     Nome = "Rafael Almeida",
            //     CEP = "99999999",
            //     Cidade = "Blumenau",
            //     Estado = "SC",
            //     Telefone = "99000001111"
            // };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Teste 1",
                    CEP = "99999999",
                    Cidade = "Blumenau",
                    Estado = "SC",
                    Telefone = "99000001111",
                },
                new Cliente
                {
                    Nome = "Teste 2",
                    CEP = "99999999",
                    Cidade = "Blumenau",
                    Estado = "SC",
                    Telefone = "99000001111",
                },
            };

            using var db = new Data.ApplicationContext();

            //db.AddRange(produto, cliente);
            db.Clientes.AddRange(listaClientes);
            //db.Set<Cliente>().AddRange(listaClientes);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new Data.ApplicationContext();

            //var consultaPorSintaxen = (from c in db.Clientes where c.Id>0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                .AsNoTracking()
                .Where(p=>p.Id > 0)
                .OrderBy(p=>p.Id)
                .ToList();
            foreach(var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                //db.Clientes.Find(cliente.Id);
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }

        private static void CadastraPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClientId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                StatusPedido = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10,
                    }
                }
            };

            db.Pedidos.Add(pedido);

            db.SaveChanges();
        }

        public static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();

            var pedidos = db
            .Pedidos
            .Include(p=>p.Itens)
                .ThenInclude(p=>p.Produto)
            .ToList();

            Console.WriteLine(pedidos.Count);
        }

        private static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.Find(1);

            cliente.Nome = "Cliente Alterado Passo 2";
            //db.Clientes.Update(cliente); //FORMA 1

            /* FORMA 2
            var clienteDesconectado = new //Quando um registro não vem diretamente do banco
            {
                Nome = "Cliente Desconectado",
                Telefone = "47999990000"
            };
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
            */

            /* FORMA 3
            var clienteDesconectado2 = new 
            {
                Id = 1
            };
            db.Attach(clienteDesconectado2); //Para rastrear o registro na base.
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado2);
            */

            //FORMA 4
            db.SaveChanges(); //Recomendado utilizar apenas o SaveChanges
        }

        private static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();

            //var cliente = db.Clientes.Find(1);
            
            //db.Clientes.Remove(cliente);
            //db.Remove(cliente);
            //db.Entry(cliente).State = EntityState.Deleted;

            var cliente = new Cliente {Id = 3};
            db.Entry(cliente).State = EntityState.Deleted;


            db.SaveChanges();
        }
    }
}
