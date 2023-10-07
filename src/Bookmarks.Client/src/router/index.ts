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
      component: BookmarksView,
    },
    {
      path: '/add',
      name: 'add',
      component: AddView
    },
    {
      path: '/edit/:id',
      name: 'edit',
      component: EditView
    },
    {
      path: '/delete/:id',
      name: 'delete',
      component: DeleteView
    }
  ]
})

export default router
