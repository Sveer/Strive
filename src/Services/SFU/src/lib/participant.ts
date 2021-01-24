import Connection from './connection';
import { ProducerLink, ProducerSource } from './types';

export class Participant {
   constructor(public participantId: string) {}

   public connections: Connection[] = [];

   producers: { [key in ProducerSource]?: ProducerLink } = {};

   public getReceiveConnection(): Connection | undefined {
      // find first consuming transport
      for (const conn of this.connections) {
         const receiveTransport = conn.getReceiveTransport();
         if (receiveTransport) return conn;
      }

      return undefined;
   }
}
