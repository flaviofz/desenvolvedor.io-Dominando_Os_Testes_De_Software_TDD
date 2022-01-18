using System;
using System.Collections.Generic;
using NerdStore.Core.Messages;

namespace NerdStore.Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        //private List<Event> _notificacoes;
        //public IReadOnlyCollection<Event> Notificacoes => _notificacoes?.AsReadOnly();

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        //public void AdicionarEvento(Event evento)
        //{
        //    _notificacoes = _notificacoes ?? new List<Event>();
        //    _notificacoes.Add(evento);
        //}

        //public void RemoverEvento(Event eventItem)
        //{
        //    _notificacoes?.Remove(eventItem);
        //}

        //public void LimparEventos()
        //{
        //    _notificacoes?.Clear();
        //}
    }
}