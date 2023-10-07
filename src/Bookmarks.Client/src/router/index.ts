import { createRouter, createWebHistory } from 'vue-router'
import BookmarksView from '../views/BookmarksView.vue'
import AddView from '../views/AddView.vue'
import EditView from '../views/EditView.vue';
import DeleteView from '../views/DeleteView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'bookmarks',
      component: BookmarksView
    },
    {
      path: '/add',
      name: 'add',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: AddView
    },
    {
      path: '/edit/:id',
      name: 'edit',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: EditView
    },
    {
      path: '/delete/:id',
      name: 'delete',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: DeleteView
    }
  ]
})

export default router
